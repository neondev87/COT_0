// Un emote comprado en la tienda tal como se guarda en compras.json (sin lógica)
namespace COT_0.Host.Models;

/// <summary>Lo necesario para volver a pintarlo en el chat sin pedirlo otra vez a 7TV; fecha en UTC.</summary>
public class PurchasedEmote
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    /// <summary>Imagen chica del CDN de 7TV (webp) y su alternativa (gif).</summary>
    public string Url { get; set; } = string.Empty;
    public string? FallbackUrl { get; set; }
    public string? StillUrl { get; set; }
    public string LargeUrl { get; set; } = string.Empty;
    public string? LargeFallbackUrl { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public bool Animated { get; set; }
    public DateTime PurchasedAt { get; set; }
}
