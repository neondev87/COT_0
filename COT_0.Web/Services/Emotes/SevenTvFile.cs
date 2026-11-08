namespace COT_0.Web.Services.Emotes;

/// <summary>Un archivo de imagen del emote en el CDN de 7TV (tamaño y formato).</summary>
internal sealed record SevenTvFile(string Name, string? StaticName, int Width, int Height, string Format);
