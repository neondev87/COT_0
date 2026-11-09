using COT_0.Core.Models;

namespace COT_0.Web.Services.Emotes;

/// <summary>Traduce el JSON de 7TV a RoomEmote usando los nombres de archivo que manda la API (no se arman a mano)</summary>
internal static class SevenTvMapper
{
    public static IReadOnlyList<RoomEmote> Map(SevenTvSet set, ChatOptions options) =>
        (set.Emotes ?? [])
            .Select(e => Map(e, options))
            .OfType<RoomEmote>()
            .DistinctBy(e => e.Name, StringComparer.Ordinal)
            .ToList();

    // Resultados de búsqueda: varios emotes distintos pueden llamarse igual, así que se separan por ID
    public static IReadOnlyList<RoomEmote> MapMany(IEnumerable<SevenTvActiveEmote> emotes, ChatOptions options) =>
        emotes.Select(e => Map(e, options)).OfType<RoomEmote>().DistinctBy(e => e.Id).ToList();

    private static RoomEmote? Map(SevenTvActiveEmote emote, ChatOptions options)
    {
        if (string.IsNullOrWhiteSpace(emote.Name) || emote.Data?.Host is not { Files: { } files } host) return null;
        if (ToCdnUrl(host.Url, options.SevenTvCdnHost) is not { } baseUrl) return null;

        var small = FindFile(files, options.EmoteScale, options.EmoteFormat);
        if (small is null || small.Width <= 0 || small.Height <= 0) return null;

        var large = FindFile(files, options.EmoteLargeScale, options.EmoteFormat) ?? small;
        return new RoomEmote(emote.Id, emote.Name, small.Width, small.Height, emote.Data.Animated,
            ToSources(baseUrl, files, small, options), ToSources(baseUrl, files, large, options));
    }

    private static EmoteSources ToSources(string baseUrl, List<SevenTvFile> files, SevenTvFile file, ChatOptions options)
    {
        var scale = Path.GetFileNameWithoutExtension(file.Name);
        var fallback = FindFile(files, scale, options.EmoteFallbackFormat);
        var still = string.IsNullOrEmpty(file.StaticName) ? null : $"{baseUrl}/{file.StaticName}";

        return new EmoteSources($"{baseUrl}/{file.Name}", still, fallback is null ? null : $"{baseUrl}/{fallback.Name}",
            $"image/{options.EmoteFormat.ToLowerInvariant()}");
    }

    private static SevenTvFile? FindFile(List<SevenTvFile> files, string scale, string format) =>
        files.FirstOrDefault(f => f.Format.Equals(format, StringComparison.OrdinalIgnoreCase)
            && Path.GetFileNameWithoutExtension(f.Name).Equals(scale, StringComparison.OrdinalIgnoreCase));

    // La API manda "//cdn.7tv.app/emote/{id}": se pasa a https y solo se acepta el host del CDN configurado
    private static string? ToCdnUrl(string url, string cdnHost)
    {
        var candidate = url.StartsWith("//", StringComparison.Ordinal) ? $"https:{url}" : url;
        return Uri.TryCreate(candidate, UriKind.Absolute, out var uri)
            && uri.Scheme == Uri.UriSchemeHttps
            && uri.Host.Equals(cdnHost, StringComparison.OrdinalIgnoreCase)
            ? uri.AbsoluteUri.TrimEnd('/')
            : null;
    }
}
