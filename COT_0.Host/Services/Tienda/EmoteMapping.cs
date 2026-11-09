// Pasa un emote de 7TV (RoomEmote) a lo que se guarda en compras.json y de regreso
using COT_0.Core.Models;
using COT_0.Host.Models;

namespace COT_0.Host.Services.Tienda;

/// <summary>Guarda las URL del CDN tal como llegaron, así el chat lo pinta igual que uno del set.</summary>
public static class EmoteMapping
{
    public static PurchasedEmote ToPurchased(RoomEmote emote, DateTime purchasedAt) => new()
    {
        Id = emote.Id,
        Name = emote.Name,
        Url = emote.Small.Url,
        FallbackUrl = emote.Small.FallbackUrl,
        StillUrl = emote.Small.StillUrl,
        LargeUrl = emote.Large.Url,
        LargeFallbackUrl = emote.Large.FallbackUrl,
        MimeType = emote.Small.MimeType,
        Width = emote.Width,
        Height = emote.Height,
        Animated = emote.Animated,
        PurchasedAt = purchasedAt,
    };

    public static RoomEmote ToRoom(PurchasedEmote emote) => new(
        emote.Id, emote.Name, emote.Width, emote.Height, emote.Animated,
        new EmoteSources(emote.Url, emote.StillUrl, emote.FallbackUrl, emote.MimeType),
        new EmoteSources(string.IsNullOrEmpty(emote.LargeUrl) ? emote.Url : emote.LargeUrl, null,
            emote.LargeFallbackUrl ?? emote.FallbackUrl, emote.MimeType));

    /// <summary>La miniatura que pinta el Host: gif si hay (WinUI lo decodifica sin códecs extra), si no la webp.</summary>
    public static string ThumbnailUrl(PurchasedEmote emote) => emote.FallbackUrl ?? emote.Url;

    public static string ThumbnailUrl(RoomEmote emote) => emote.Small.FallbackUrl ?? emote.Small.Url;
}
