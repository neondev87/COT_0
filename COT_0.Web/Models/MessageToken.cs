using COT_0.Core.Models;

namespace COT_0.Web.Models;

// Fragmento para pintar: texto plano, link (Url) o emote. El texto lo escapa Blazor al renderizar.
public sealed record MessageToken(string Text, string? Url = null, RoomEmote? Emote = null);

// Mensaje ya partido; si solo tiene emotes (y espacios) se muestra más grande
public sealed record FormattedMessage(IReadOnlyList<MessageToken> Tokens)
{
    public bool IsEmoteOnly { get; } =
        Tokens.Any(t => t.Emote is not null) && Tokens.All(t => t.Emote is not null || string.IsNullOrWhiteSpace(t.Text));
}
