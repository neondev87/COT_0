namespace COT_0.Web.Services.Emotes;

/// <summary>Total de coincidencias en 7TV y los emotes de esta página.</summary>
internal sealed record SevenTvEmoteSearch(int Count, List<SevenTvSearchItem>? Items);
