using COT_0.Core.Models;

namespace COT_0.Web.Services.Emotes;

// Respuesta de 7TV ya traducida a emotes de la sala; HttpCode solo cuando 7TV respondió con error
public sealed record EmoteSetResult(EmoteSetStatus Status, string? Name, IReadOnlyList<RoomEmote> Emotes,
    EmoteSetError Error = EmoteSetError.None, int? HttpCode = null)
{
    public static EmoteSetResult Fail(EmoteSetError error, int? httpCode = null) =>
        new(error is EmoteSetError.NotFound or EmoteSetError.InvalidId ? EmoteSetStatus.NotFound : EmoteSetStatus.Failed,
            null, [], error, httpCode);
}
