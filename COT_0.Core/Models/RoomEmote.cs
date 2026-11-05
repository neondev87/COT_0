namespace COT_0.Core.Models;

// Emote de un set de 7TV. Las imágenes se cargan directo del CDN (no se guardan en disco)
public sealed record RoomEmote(string Id, string Name, int Width, int Height, bool Animated, EmoteSources Small, EmoteSources Large);

// URLs de un tamaño: animada (webp), primer cuadro (para prefers-reduced-motion) y alternativa (gif)
public sealed record EmoteSources(string Url, string? StillUrl, string? FallbackUrl, string MimeType);
