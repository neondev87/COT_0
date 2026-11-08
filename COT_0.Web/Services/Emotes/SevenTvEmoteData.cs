namespace COT_0.Web.Services.Emotes;

/// <summary>Datos del emote en el JSON de 7TV: si es animado y dónde están sus imágenes.</summary>
internal sealed record SevenTvEmoteData(bool Animated, SevenTvHost? Host);
