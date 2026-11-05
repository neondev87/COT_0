using COT_0.Core.Models;

namespace COT_0.Web.Services.Emotes;

// Foto fija de los emotes del set: activos (con índice por nombre) y quitados por el host
internal sealed record EmoteList(IReadOnlyList<RoomEmote> Active, IReadOnlyList<RoomEmote> Removed, IReadOnlyDictionary<string, RoomEmote> ByName)
{
    public static EmoteList Empty { get; } = new([], [], new Dictionary<string, RoomEmote>());

    // Los nombres de 7TV distinguen mayúsculas (como en Kick/Twitch)
    public static EmoteList From(IReadOnlyList<RoomEmote> all, IReadOnlySet<string> removedIds)
    {
        var active = all.Where(e => !removedIds.Contains(e.Id)).ToList();
        var removed = all.Where(e => removedIds.Contains(e.Id)).ToList();
        return new EmoteList(active, removed, active.ToDictionary(e => e.Name, StringComparer.Ordinal));
    }
}
