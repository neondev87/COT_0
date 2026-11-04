using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Emotes;

// Cliente tipado: BaseAddress y Timeout salen de ChatOptions (ver EmoteRegistration)
public sealed class SevenTvClient(HttpClient http, IOptions<ChatOptions> options, ILogger<SevenTvClient> logger) : ISevenTvClient
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    public async Task<EmoteSetResult> GetEmoteSetAsync(string setId, CancellationToken ct = default)
    {
        var o = options.Value;
        try
        {
            using var response = await http.GetAsync(string.Format(o.SevenTvEmoteSetPath, Uri.EscapeDataString(setId)), ct);

            // 400 = ID mal formado, 404 = no existe; otro código = error de 7TV
            if (response.StatusCode == HttpStatusCode.BadRequest) return EmoteSetResult.Fail(EmoteSetError.InvalidId, 400);
            if (response.StatusCode == HttpStatusCode.NotFound) return EmoteSetResult.Fail(EmoteSetError.NotFound, 404);
            if (!response.IsSuccessStatusCode) return Failed(setId, null, EmoteSetError.HttpError, (int)response.StatusCode);

            var set = await response.Content.ReadFromJsonAsync<SevenTvSet>(Json, ct);
            return set is null
                ? Failed(setId, null, EmoteSetError.BadJson)
                : new EmoteSetResult(EmoteSetStatus.Ready, set.Name, SevenTvMapper.Map(set, o));
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            return Failed(setId, ex, EmoteSetError.Timeout);
        }
        catch (HttpRequestException ex)
        {
            // DNS, TLS, conexión cortada o proxy
            return Failed(setId, ex, EmoteSetError.Network);
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            return Failed(setId, ex, EmoteSetError.BadJson);
        }
    }

    private EmoteSetResult Failed(string setId, Exception? ex, EmoteSetError error, int? httpCode = null)
    {
        logger.LogWarning(ex, "No se pudo leer el set de emotes {SetId} de 7TV: {Error} {HttpCode}", setId, error, httpCode);
        return EmoteSetResult.Fail(error, httpCode);
    }
}
