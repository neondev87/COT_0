namespace COT_0.Core.Models;

/// <summary>URLs de un tamaño: animada (webp), primer cuadro (para prefers-reduced-motion) y alternativa (gif)</summary>
public sealed record EmoteSources(string Url, string? StillUrl, string? FallbackUrl, string MimeType);
