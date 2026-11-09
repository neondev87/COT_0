namespace COT_0.Web.Services.Emotes;

/// <summary>Cliente de la API pública v3 de 7TV (GET /emote-sets/{id} y búsqueda por GraphQL)</summary>
public interface ISevenTvClient
{
    Task<EmoteSetResult> GetEmoteSetAsync(string setId, CancellationToken ct = default);

    // Una página del catálogo completo (page empieza en 1); query vacío = los más populares
    Task<EmoteCatalogPage> SearchEmotesAsync(string? query, int page, int pageSize, CancellationToken ct = default);
}
