// Cupo de la tienda: Store:DailyLimit emotes y, al llegar al límite, Store:CooldownHours de espera; todo en UTC y leído de compras.json
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Tienda;

/// <summary>
/// La ventana empieza con la primera compra. Al llegar a DailyLimit se guarda LimitReachedAt (la hora de esa compra)
/// y el cupo vuelve completo CooldownHours después; si no llegaste al límite, vuelve completo CooldownHours después de la
/// primera compra. Siempre se calcula contra la hora actual al leer el archivo, sin contadores en memoria.
/// </summary>
public static class QuotaCalculator
{
    private static StoreSettings Store => AppConfig.Settings.Store;

    public static TimeSpan Cooldown => TimeSpan.FromHours(Store.CooldownHours);

    public static QuotaInfo For(UserPurchases? user, DateTime nowUtc)
    {
        var limit = Store.DailyLimit;
        if (user?.WindowStart is not { } start || IsExpired(user, nowUtc)) return new QuotaInfo(0, limit, limit, null);

        var used = Used(user, start);
        return new QuotaInfo(used, Math.Max(0, limit - used), limit, ResetsAt(user, start));
    }

    /// <summary>Cierra la ventana vencida y completa LimitReachedAt si falta (compras.json de antes); se guarda con la siguiente compra.</summary>
    public static void Refresh(UserPurchases user, DateTime nowUtc)
    {
        if (user.WindowStart is not { } start) return;
        if (IsExpired(user, nowUtc))
        {
            user.WindowStart = null;
            user.LimitReachedAt = null;
            return;
        }
        user.LimitReachedAt ??= ReachedAt(user, start);
    }

    /// <summary>Después de agregar compras: si ya se llegó al límite, guarda la hora en que empieza la espera.</summary>
    public static void MarkLimit(UserPurchases user, DateTime nowUtc)
    {
        if (user.WindowStart is { } start && user.LimitReachedAt is null && Used(user, start) >= Store.DailyLimit)
            user.LimitReachedAt = nowUtc;
    }

    /// <summary>Lo que falta para que se reinicie (cero si ya se reinició).</summary>
    public static TimeSpan Wait(QuotaInfo quota, DateTime nowUtc) =>
        quota.ResetsAt is { } at && at > nowUtc ? at - nowUtc : TimeSpan.Zero;

    private static int Used(UserPurchases user, DateTime start) => user.Emotes.Count(e => e.PurchasedAt >= start);

    private static DateTime ResetsAt(UserPurchases user, DateTime start) =>
        (user.LimitReachedAt ?? ReachedAt(user, start) ?? start) + Cooldown;

    // Hora de la compra que completó el límite (para archivos sin LimitReachedAt)
    private static DateTime? ReachedAt(UserPurchases user, DateTime start)
    {
        var inWindow = user.Emotes.Where(e => e.PurchasedAt >= start).OrderBy(e => e.PurchasedAt).ToList();
        return inWindow.Count >= Store.DailyLimit ? inWindow[Store.DailyLimit - 1].PurchasedAt : null;
    }

    private static bool IsExpired(UserPurchases user, DateTime nowUtc) => nowUtc >= ResetsAt(user, user.WindowStart!.Value);
}
