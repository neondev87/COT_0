using COT_0.Core.Models;

namespace COT_0.Web.Services.Emotes;

// Búsqueda del picker: primero los que empiezan con el texto, después los que lo contienen, los cortos antes
internal static class EmoteSearch
{
    public static IReadOnlyList<RoomEmote> Find(IReadOnlyList<RoomEmote> emotes, string? query, int max)
    {
        if (string.IsNullOrWhiteSpace(query)) return emotes.Take(max).ToList();

        var text = query.Trim();
        return emotes
            .Where(e => e.Name.Contains(text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(e => e.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(e => e.Name.Length)
            .ThenBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
            .Take(max)
            .ToList();
    }
}
