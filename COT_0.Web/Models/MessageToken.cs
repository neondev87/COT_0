using COT_0.Core.Models;

namespace COT_0.Web.Models;

/// <summary>Fragmento para pintar: texto plano, link (Url) o emote. El texto lo escapa Blazor al renderizar.</summary>
public sealed record MessageToken(string Text, string? Url = null, RoomEmote? Emote = null);
