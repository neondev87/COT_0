using COT_0.Core.Models;

namespace COT_0.Web.Services.Emotes;

/// <summary>Una página de la búsqueda en el catálogo de 7TV; Error = None si respondió bien</summary>
public sealed record EmoteCatalogPage(IReadOnlyList<RoomEmote> Emotes, int Total, EmoteSetError Error = EmoteSetError.None, int? HttpCode = null)
{
    public bool IsOk => Error == EmoteSetError.None;

    public static EmoteCatalogPage Fail(EmoteSetError error, int? httpCode = null) => new([], 0, error, httpCode);
}
