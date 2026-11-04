namespace COT_0.Web.Services.Emotes;

public enum EmoteSetStatus
{
    Off,
    Loading,
    Ready,
    NotFound,
    Failed,
}

// Motivo concreto cuando el set no se pudo cargar (el Host lo muestra tal cual)
public enum EmoteSetError
{
    None,
    InvalidId,
    NotFound,
    Timeout,
    Network,
    HttpError,
    BadJson,
}

// Respuesta de 7TV ya traducida a emotes de la sala; HttpCode solo cuando 7TV respondió con error
public sealed record EmoteSetResult(EmoteSetStatus Status, string? Name, IReadOnlyList<RoomEmote> Emotes,
    EmoteSetError Error = EmoteSetError.None, int? HttpCode = null)
{
    public static EmoteSetResult Fail(EmoteSetError error, int? httpCode = null) =>
        new(error is EmoteSetError.NotFound or EmoteSetError.InvalidId ? EmoteSetStatus.NotFound : EmoteSetStatus.Failed,
            null, [], error, httpCode);
}
