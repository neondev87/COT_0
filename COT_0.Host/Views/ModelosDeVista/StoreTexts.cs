// Textos armados de la tienda: reglas del cupo, contador, bloqueo con HH:MM:SS de espera y errores de 7TV
using System.Globalization;
using COT_0.Core.Models;
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Textos;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Un solo lugar para los formatos; los números salen del cupo calculado y de appsettings.json.</summary>
public static class StoreTexts
{
    private static CultureInfo Culture => CultureInfo.CurrentCulture;

    private static StoreSettings Store => AppConfig.Settings.Store;

    public static string Format(string key, params object[] values) => string.Format(Culture, UiText.Get(key), values);

    /// <summary>"Puedes comprar 10 emotes cada 24 horas" (Store:DailyLimit y Store:CooldownHours).</summary>
    public static string Rules() => Format("Text.Store.Shop.Rules", Store.DailyLimit, Store.CooldownHours);

    /// <summary>"Te quedan X de 10".</summary>
    public static string Quota(QuotaInfo quota) => Format("Text.Store.Shop.Quota", quota.Remaining, quota.Limit);

    /// <summary>HH:MM:SS (Store:WaitFormat); las horas pueden pasar de 24 si cambias CooldownHours.</summary>
    public static string Wait(TimeSpan wait)
    {
        var seconds = (long)Math.Max(0, Math.Ceiling(wait.TotalSeconds));
        return string.Format(Culture, Store.WaitFormat, seconds / 3600, seconds / 60 % 60, seconds % 60);
    }

    /// <summary>"Ya compraste tus 10 emotes. Podrás comprar más en HH:MM:SS".</summary>
    public static string Blocked(int limit, string wait) => Format("Text.Store.Shop.Blocked", limit, wait);

    /// <summary>Motivo del fallo al buscar en 7TV (timeout, red, código HTTP, JSON).</summary>
    public static string CatalogError(EmoteSetError error, int? httpCode) =>
        Format($"Text.Emotes.Error.{error}", httpCode ?? 0);
}
