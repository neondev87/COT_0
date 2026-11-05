using COT_0.Core.Services;
using System.Text.RegularExpressions;
using COT_0.Web.Models;

namespace COT_0.Web.Services;

public partial class MessageFormatter(IRoomEmotes emotes) : IMessageFormatter
{
    // Signos que suelen cerrar una oración y no forman parte del link
    private static readonly char[] TrailingPunctuation = ['.', ',', ';', ':', '!', '?', ')', ']', '}', '\'', '"'];

    // Como en Kick/7TV: una palabra igual al nombre de un emote activo se muestra como imagen.
    // Si el host lo quita, Find no lo encuentra y queda como texto (también en mensajes viejos)
    public FormattedMessage Format(string text)
    {
        var tokens = new List<MessageToken>();
        var pending = 0;

        foreach (Match word in WordPattern().Matches(text))
        {
            if (emotes.Find(word.Value) is not { } emote) continue;

            AddLinks(tokens, text[pending..word.Index]);
            tokens.Add(new MessageToken(word.Value, Emote: emote));
            pending = word.Index + word.Length;
        }

        AddLinks(tokens, text[pending..]);
        return new FormattedMessage(tokens);
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

    [GeneratedRegex(@"\S+")]
    private static partial Regex WordPattern();

    [GeneratedRegex(@"\b(?:https?://|www\.)[^\s<>""]+", RegexOptions.IgnoreCase)]
    private static partial Regex LinkPattern();
}
