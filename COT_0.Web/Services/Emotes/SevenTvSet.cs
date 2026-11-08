namespace COT_0.Web.Services.Emotes;

/// <summary>Lo mínimo del JSON de 7TV v3 (EmoteSetModel → ActiveEmoteModel → EmotePartialModel → ImageHost)</summary>
internal sealed record SevenTvSet(string Id, string Name, List<SevenTvActiveEmote>? Emotes);
