namespace COT_0.Web.Services.Emotes;

/// <summary>Emote en el JSON de 7TV (resultado de búsqueda ya convertido).</summary>
internal sealed record SevenTvActiveEmote(string Id, string Name, SevenTvEmoteData? Data);
