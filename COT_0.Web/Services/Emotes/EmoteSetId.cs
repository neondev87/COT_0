using System.Text.RegularExpressions;

namespace COT_0.Web.Services.Emotes;

/// <summary>Saca el ID de un set de 7TV de un ID suelto o de una URL (https://7tv.app/emote-sets/{id})</summary>
public static partial class EmoteSetId
{
    private const string SetsSegment = "emote-sets";

    public static string? Parse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        var value = input.Trim();
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var at = Array.FindIndex(segments, s => s.Equals(SetsSegment, StringComparison.OrdinalIgnoreCase));
            value = at >= 0 && at + 1 < segments.Length ? segments[at + 1] : segments.LastOrDefault() ?? string.Empty;
        }

        return IdPattern().IsMatch(value) ? value : null;
    }

    // ULID (26) u ObjectId viejo (24); "global" es el set global de 7TV
    [GeneratedRegex("^(?:global|[0-9A-Za-z]{24,26})$")]
    private static partial Regex IdPattern();
}
