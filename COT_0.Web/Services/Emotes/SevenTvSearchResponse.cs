namespace COT_0.Web.Services.Emotes;

/// <summary>Respuesta de GraphQL de 7TV: { data: { emotes: { count, items } } }</summary>
internal sealed record SevenTvSearchResponse(SevenTvSearchData? Data);
