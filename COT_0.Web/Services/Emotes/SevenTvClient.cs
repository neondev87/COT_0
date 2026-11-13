using System.Net.Http.Json;
using System.Text.Json;
using COT_0.Core.Models;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Emotes;

/// <summary>Cliente tipado: BaseAddress y Timeout salen de ChatOptions (ver EmoteRegistration)</summary>
public sealed class SevenTvClient(HttpClient http, IOptions<ChatOptions> options, ILogger<SevenTvClient> logger) : ISevenTvClient
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    // POST de GraphQL; timeout, red, código HTTP o JSON malo regresan un error sin tumbar a quien llama
    public async Task<EmoteCatalogPage> SearchEmotesAsync(string? query, int page, int pageSize, CancellationToken ct = default)
    {
        try
        {
            using var response = await http.PostAsJsonAsync(options.Value.SevenTvGqlPath, SevenTvSearchQuery.Body(query, page, pageSize), ct);
            if (!response.IsSuccessStatusCode) return SearchFailed(query, null, EmoteSetError.HttpError, (int)response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<SevenTvSearchResponse>(Json, ct);
            if (body?.Data?.Emotes is not { } found) return SearchFailed(query, null, EmoteSetError.BadJson);

            var items = (found.Items ?? []).Select(i => i.ToActive());
            return new EmoteCatalogPage(SevenTvMapper.MapMany(items, options.Value), found.Count);
        }
        catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
        {
            return SearchFailed(query, ex, EmoteSetError.Timeout);
        }
        catch (HttpRequestException ex)
        {
            return SearchFailed(query, ex, EmoteSetError.Network);
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            return SearchFailed(query, ex, EmoteSetError.BadJson);
        }
    }

    private EmoteCatalogPage SearchFailed(string? query, Exception? ex, EmoteSetError error, int? httpCode = null)
    {
        logger.LogWarning(ex, "No se pudo buscar \"{Query}\" en 7TV: {Error} {HttpCode}", query, error, httpCode);
        return EmoteCatalogPage.Fail(error, httpCode);
    }
}
