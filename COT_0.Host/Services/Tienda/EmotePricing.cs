// Precio de muestra de un emote, en dólares, sacado de su id: siempre sale igual para el mismo emote
using System.Globalization;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Tienda;

/// <summary>
/// Solo es un detalle visual: no hay saldo, cobro ni mínimo. Usa FNV-1a (y no string.GetHashCode, que cambia en cada
/// arranque) para que el precio sea estable entre aperturas de la tienda y entre equipos.
/// </summary>
public static class EmotePricing
{
    private const uint FnvOffset = 2166136261;
    private const uint FnvPrime = 16777619;
    private const int PercentBase = 100;

    private static StoreSettings Store => AppConfig.Settings.Store;

    public static decimal PriceOf(string emoteId)
    {
        var hash = Hash(emoteId);
        var premium = hash % PercentBase < Store.PremiumPercent;
        var (min, max) = premium ? (Store.PremiumMinCents, Store.PremiumMaxCents) : (Store.PriceMinCents, Store.PriceMaxCents);
        var cents = min + (int)(hash / PercentBase % (uint)(max - min + 1));
        return cents / 100m;
    }

    public static decimal TotalOf(IEnumerable<string> emoteIds) => emoteIds.Sum(PriceOf);

    /// <summary>"US$ 3.27" (Store:PriceFormat), siempre con punto decimal.</summary>
    public static string Format(decimal price) => string.Format(CultureInfo.InvariantCulture, Store.PriceFormat, price);

    private static uint Hash(string text)
    {
        var hash = FnvOffset;
        foreach (var c in text)
        {
            hash ^= c;
            hash *= FnvPrime;
        }
        return hash;
    }
}
