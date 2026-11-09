// Textos armados de la tienda: cupo, bloqueo con horas de espera y errores de 7TV
using System.Globalization;
using COT_0.Core.Models;
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Tienda;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Un solo lugar para los formatos; los números salen del cupo calculado y de appsettings.json.</summary>
public static class StoreTexts
{
    private static CultureInfo Culture => CultureInfo.CurrentCulture;

    public static string Format(string key, params object[] values) => string.Format(Culture, UiText.Get(key), values);

    /// <summary>"Te quedan X de 10".</summary>
    public static string Quota(QuotaInfo quota) => Format("Text.Store.Shop.Quota", quota.Remaining, quota.Limit);

    /// <summary>"Ya compraste tus 10 emotes. Podrás comprar más en HH:MM horas", leyendo la ventana de compras.json.</summary>
    public static string Blocked(QuotaInfo quota)
    {
        var minutes = (int)Math.Ceiling(QuotaCalculator.Wait(quota, DateTime.UtcNow).TotalMinutes);
        var wait = string.Format(Culture, AppConfig.Settings.Store.WaitFormat, minutes / 60, minutes % 60);
        return Format("Text.Store.Shop.Blocked", quota.Limit, wait);
    }

    /// <summary>Mismo motivo que en el set de 7TV (timeout, red, código HTTP, JSON).</summary>
    public static string CatalogError(EmoteSetError error, int? httpCode) =>
        Format($"Text.Emotes.Error.{error}", httpCode ?? 0);
}
