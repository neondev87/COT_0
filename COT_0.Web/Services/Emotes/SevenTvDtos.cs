namespace COT_0.Web.Services.Emotes;

// Lo mínimo del JSON de 7TV v3 (EmoteSetModel → ActiveEmoteModel → EmotePartialModel → ImageHost)
internal sealed record SevenTvSet(string Id, string Name, List<SevenTvActiveEmote>? Emotes);

internal sealed record SevenTvActiveEmote(string Id, string Name, SevenTvEmoteData? Data);

internal sealed record SevenTvEmoteData(bool Animated, SevenTvHost? Host);

internal sealed record SevenTvHost(string Url, List<SevenTvFile>? Files);

internal sealed record SevenTvFile(string Name, string? StaticName, int Width, int Height, string Format);
