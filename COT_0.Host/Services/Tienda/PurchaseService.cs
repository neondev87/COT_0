// Compras de la tienda por usuario: cupo, emotes comprados y tickets, todo guardado en compras.json
using COT_0.Core.Models;
using COT_0.Host.Models;
using COT_0.Host.Services.Usuarios;

namespace COT_0.Host.Services.Tienda;

/// <summary>
/// COMPRAR es gratis: solo gasta cupo (QuotaCalculator). Una compra a la vez (lee, cambia y guarda el archivo),
/// cada una con su ticket de número consecutivo. Avisa con Purchased para que el chat lo agregue en vivo.
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

    public async Task<PurchaseResult> BuyAsync(string? userName, RoomEmote emote)
    {
        var limit = new QuotaInfo(0, 0, 0, null);
        if (string.IsNullOrEmpty(userName)) return new PurchaseResult(PurchaseOutcome.NoUser, limit, null);

        PurchaseResult result;
        await _gate.WaitAsync();
        try
        {
            result = await BuyLockedAsync(userName, emote);
        }
        finally
        {
            _gate.Release();
        }

        if (result.Outcome == PurchaseOutcome.Bought) Purchased?.Invoke(userName);
        return result;
    }

    private async Task<PurchaseResult> BuyLockedAsync(string userName, RoomEmote emote)
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
        if (user.Emotes.Any(e => e.Id == emote.Id)) return new PurchaseResult(PurchaseOutcome.AlreadyOwned, quota, null);

        user.WindowStart ??= now;
        var purchased = EmoteMapping.ToPurchased(emote, now);
        user.Emotes.Add(purchased);

        quota = QuotaCalculator.For(user, now);
        var ticket = new PurchaseTicket
        {
            Number = ++data.LastTicketNumber,
            CreatedAt = now,
            Emotes = [purchased],
            Remaining = quota.Remaining,
            ResetsAt = quota.ResetsAt ?? now + QuotaCalculator.Cooldown,
        };
        user.Tickets.Add(ticket);

        await store.SaveAsync(data);
        return new PurchaseResult(PurchaseOutcome.Bought, quota, ticket);
    }

    private static UserPurchases? Find(PurchasesData data, string userName) =>
        data.Users.FirstOrDefault(u => string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase));
}
