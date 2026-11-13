// Compras de la tienda por usuario: cupo, emotes comprados y tickets, todo guardado en compras.json
using COT_0.Core.Models;
using COT_0.Host.Models;
using COT_0.Host.Services.Usuarios;

namespace COT_0.Host.Services.Tienda;

/// <summary>
/// COMPRAR es gratis: solo gasta cupo (QuotaCalculator). Una compra a la vez (lee, cambia y guarda el archivo),
/// cada una (carrito o un emote) con un ticket de número consecutivo. Avisa con Purchased para que el chat lo agregue en vivo.
/// </summary>
public class PurchaseService(PurchaseStore store)
{
    private readonly SemaphoreSlim _gate = new(1, 1);

    /// <summary>Se compró algo para ese usuario (puede llegar desde otro hilo).</summary>
    public event Action<string>? Purchased;

    /// <summary>true una sola vez si compras.json estaba dañado y se recreó vacío.</summary>
    public bool TakeRecreatedNotice() => store.TakeRecreatedNotice();

    /// <summary>Las compras de ese usuario (vacías si nunca compró), con la ventana vencida ya cerrada.</summary>
    public async Task<UserPurchases> GetAsync(string userName)
    {
        var user = Find(await store.LoadAsync(), userName) ?? new UserPurchases { UserName = userName };
        QuotaCalculator.Refresh(user, DateTime.UtcNow);
        return user;
    }

    /// <summary>Compras de todos los usuarios para el punto de venta: solo en modo ADMIN (vacío si no).</summary>
    public async Task<PurchasesData> GetAllAsync() => ActiveUser.IsAdmin ? await store.LoadAsync() : new PurchasesData();

    public async Task<QuotaInfo> GetQuotaAsync(string userName) => QuotaCalculator.For(await GetAsync(userName), DateTime.UtcNow);

    /// <summary>
    /// Una sola compra con un solo ticket para todos los emotes (CARRITO) o para uno (UNO POR UNO).
    /// Los que ya tenías se saltan; si no caben en el cupo que queda, no se compra ninguno.
    /// </summary>
    public async Task<PurchaseResult> BuyAsync(string? userName, IReadOnlyList<RoomEmote> emotes)
    {
        var limit = new QuotaInfo(0, 0, 0, null);
        if (string.IsNullOrEmpty(userName)) return new PurchaseResult(PurchaseOutcome.NoUser, limit, null);
        if (emotes.Count == 0) return new PurchaseResult(PurchaseOutcome.Empty, limit, null);

        PurchaseResult result;
        await _gate.WaitAsync();
        try
        {
            result = await BuyLockedAsync(userName, emotes);
        }
        finally
        {
            _gate.Release();
        }

        if (result.Outcome == PurchaseOutcome.Bought) Purchased?.Invoke(userName);
        return result;
    }

    private async Task<PurchaseResult> BuyLockedAsync(string userName, IReadOnlyList<RoomEmote> emotes)
    {
        var now = DateTime.UtcNow;
        var data = await store.LoadAsync();
        var user = Find(data, userName);
        if (user is null)
        {
            user = new UserPurchases { UserName = userName };
            data.Users.Add(user);
        }
        QuotaCalculator.Refresh(user, now);

        var quota = QuotaCalculator.For(user, now);
        if (quota.IsBlocked) return new PurchaseResult(PurchaseOutcome.LimitReached, quota, null);
        var fresh = emotes.DistinctBy(e => e.Id).Where(e => user.Emotes.All(owned => owned.Id != e.Id)).ToList();
        if (fresh.Count == 0) return new PurchaseResult(PurchaseOutcome.AlreadyOwned, quota, null);
        if (fresh.Count > quota.Remaining) return new PurchaseResult(PurchaseOutcome.NotEnoughQuota, quota, null);

        user.WindowStart ??= now;
        var purchased = fresh.Select(e => EmoteMapping.ToPurchased(e, now)).ToList();
        user.Emotes.AddRange(purchased);
        QuotaCalculator.MarkLimit(user, now);

        quota = QuotaCalculator.For(user, now);
        var ticket = new PurchaseTicket
        {
            Number = ++data.LastTicketNumber,
            CreatedAt = now,
            Emotes = purchased,
            Remaining = quota.Remaining,
            ResetsAt = quota.ResetsAt ?? now + QuotaCalculator.Cooldown,
        };
        user.Tickets.Add(ticket);

        await store.SaveAsync(data);
        return new PurchaseResult(PurchaseOutcome.Bought, quota, ticket);
    }

    /// <summary>
    /// ELIMINAR cuenta: se van sus compras y sus tickets. Regresa los números de sus tickets para borrar las copias
    /// de la app (los PDF que ya bajó al Escritorio se quedan).
    /// </summary>
    public async Task<IReadOnlyList<int>> RemoveUserAsync(string userName)
    {
        List<int> numbers = [];
        await ChangeAsync(data =>
        {
            numbers.AddRange(data.Users.Where(u => Same(u.UserName, userName)).SelectMany(u => u.Tickets).Select(t => t.Number));
            data.Users.RemoveAll(u => Same(u.UserName, userName));
        });
        return numbers;
    }

    /// <summary>CAMBIAR NOMBRE: sus compras siguen siendo suyas con el nombre nuevo.</summary>
    public Task RenameUserAsync(string oldName, string newName) => ChangeAsync(data =>
    {
        if (Find(data, oldName) is { } user) user.UserName = newName;
    });

    private async Task ChangeAsync(Action<PurchasesData> change)
    {
        await _gate.WaitAsync();
        try
        {
            var data = await store.LoadAsync();
            change(data);
            await store.SaveAsync(data);
        }
        finally
        {
            _gate.Release();
        }
    }

    private static bool Same(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    private static UserPurchases? Find(PurchasesData data, string userName) => data.Users.FirstOrDefault(u => Same(u.UserName, userName));
}
