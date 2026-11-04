using System.Text.RegularExpressions;
using COT_0.Core.Services;
using COT_0.Web.Models;

namespace COT_0.Web.Services;

public partial class MessageFormatter(IEmoteService emotes) : IMessageFormatter
{
    // Signos que suelen cerrar una oración y no forman parte del link
    private static readonly char[] TrailingPunctuation = ['.', ',', ';', ':', '!', '?', ')', ']', '}', '\'', '"'];

    public IReadOnlyList<MessageToken> Format(string text)
    {
        var tokens = new List<MessageToken>();

        foreach (var part in emotes.Parse(text))
        {
            if (part.Emote is null) AddLinks(tokens, part.Text);
            else tokens.Add(new MessageToken(part.Text, Emote: part.Emote));
        }

        return tokens;
    }

    private static void AddLinks(List<MessageToken> tokens, string text)
    {
        var last = 0;

        foreach (Match match in LinkPattern().Matches(text))
        {
            var link = match.Value.TrimEnd(TrailingPunctuation);
            if (ToSafeUrl(link) is not { } url) continue;

            if (match.Index > last) tokens.Add(new MessageToken(text[last..match.Index]));
            tokens.Add(new MessageToken(link, url));
            last = match.Index + link.Length;
        }

        if (last < text.Length) tokens.Add(new MessageToken(text[last..]));
    }

    // Solo http y https: nada de javascript:, data:, etc.
    private static string? ToSafeUrl(string link)
    {
        var candidate = link.StartsWith("www.", StringComparison.OrdinalIgnoreCase) ? $"https://{link}" : link;
        return Uri.TryCreate(candidate, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            ? uri.AbsoluteUri
            : null;
    }

    [GeneratedRegex(@"\b(?:https?://|www\.)[^\s<>""]+", RegexOptions.IgnoreCase)]
    private static partial Regex LinkPattern();
}
