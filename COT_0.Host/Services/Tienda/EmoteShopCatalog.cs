// Catálogo de la tienda: páginas de todo 7TV (Store:PageSize por página) con caché en memoria de lo ya cargado
using COT_0.Core.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Sala;
using COT_0.Web.Services.Emotes;
using Microsoft.Extensions.DependencyInjection;

namespace COT_0.Host.Services.Tienda;

/// <summary>
/// Usa el cliente de 7TV de la web (mismo HttpClient, Chat:EmoteRequestTimeoutSeconds = 90 s).
/// Solo guarda páginas que llegaron bien; un fallo regresa el error para el aviso con REINTENTAR.
/// </summary>
public class EmoteShopCatalog(WebServerHost webServer)
{
    private readonly Dictionary<string, EmoteCatalogPage> _cache = [];

    public static int PageSize => AppConfig.Settings.Store.PageSize;

    /// <summary>page empieza en 1; query vacío = los más populares de 7TV.</summary>
    public async Task<EmoteCatalogPage> GetPageAsync(string query, int page)
    {
        var key = $"{query.Trim().ToLowerInvariant()}|{page}";
        if (_cache.TryGetValue(key, out var cached)) return cached;

        // Fuera del hilo de la UI: si la web todavía se está armando, Services espera a que termine
        var result = await Task.Run(async () =>
        {
            try
            {
                return await webServer.Services.GetRequiredService<ISevenTvClient>().SearchEmotesAsync(query, page, PageSize);
            }
            catch (InvalidOperationException)
            {
                return EmoteCatalogPage.Fail(EmoteSetError.Network);
            }
        });

        if (result.IsOk) _cache[key] = result;
        return result;
    }
}
