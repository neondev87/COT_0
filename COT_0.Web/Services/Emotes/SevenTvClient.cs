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

            // 400 = ID mal formado, 404 = no existe
            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest) return EmoteSetResult.NotFound;
            response.EnsureSuccessStatusCode();

            var set = await response.Content.ReadFromJsonAsync<SevenTvSet>(Json, ct);
            return set is null ? EmoteSetResult.Failed : new EmoteSetResult(EmoteSetStatus.Ready, set.Name, SevenTvMapper.Map(set, o));
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException or NotSupportedException)
        {
            logger.LogWarning(ex, "No se pudo leer el set de emotes {SetId} de 7TV", setId);
            return EmoteSetResult.Failed;
        }
    }
}
