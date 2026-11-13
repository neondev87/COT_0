namespace COT_0.Web.Services.Emotes;

/// <summary>Cliente de la API pública v3 de 7TV: búsqueda por GraphQL para la tienda del Host</summary>
public interface ISevenTvClient
{
    // Una página del catálogo completo (page empieza en 1); query vacío = los más populares
    Task<EmoteCatalogPage> SearchEmotesAsync(string? query, int page, int pageSize, CancellationToken ct = default);
}
