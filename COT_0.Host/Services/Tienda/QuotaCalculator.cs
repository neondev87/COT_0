// Cupo de la tienda: Store:DailyLimit emotes cada Store:CooldownHours horas por usuario, todo en UTC
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Tienda;

/// <summary>
/// La ventana empieza con la primera compra y dura CooldownHours; pasado ese tiempo el cupo vuelve completo
/// al leerlo (al abrir la app o la tienda), sin procesos en segundo plano.
/// </summary>
public static class QuotaCalculator
{
    private static StoreSettings Store => AppConfig.Settings.Store;

    public static TimeSpan Cooldown => TimeSpan.FromHours(Store.CooldownHours);

    public static QuotaInfo For(UserPurchases? user, DateTime nowUtc)
    {
        var limit = Store.DailyLimit;
        if (user?.WindowStart is not { } start || IsExpired(start, nowUtc)) return new QuotaInfo(0, limit, limit, null);

        var used = user.Emotes.Count(e => e.PurchasedAt >= start);
        return new QuotaInfo(used, Math.Max(0, limit - used), limit, start + Cooldown);
    }

    /// <summary>Cierra la ventana vencida (se guarda con la siguiente compra).</summary>
    public static void Refresh(UserPurchases user, DateTime nowUtc)
    {
        if (user.WindowStart is { } start && IsExpired(start, nowUtc)) user.WindowStart = null;
    }

    /// <summary>Lo que falta para que se reinicie (cero si ya se reinició).</summary>
    public static TimeSpan Wait(QuotaInfo quota, DateTime nowUtc) =>
        quota.ResetsAt is { } at && at > nowUtc ? at - nowUtc : TimeSpan.Zero;

    private static bool IsExpired(DateTime start, DateTime nowUtc) => nowUtc >= start + Cooldown;
}
