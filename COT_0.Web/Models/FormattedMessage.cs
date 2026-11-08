namespace COT_0.Web.Models;

/// <summary>Mensaje ya partido; si solo tiene emotes (y espacios) se muestra más grande</summary>
public sealed record FormattedMessage(IReadOnlyList<MessageToken> Tokens)
{
    public bool IsEmoteOnly { get; } =
        Tokens.Any(t => t.Emote is not null) && Tokens.All(t => t.Emote is not null || string.IsNullOrWhiteSpace(t.Text));
}
