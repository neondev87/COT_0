namespace COT_0.Web.Services.Emotes;

public enum EmoteSetStatus
{
    Off,
    Loading,
    Ready,
    NotFound,
    Failed,
}

// Respuesta de 7TV ya traducida a emotes de la sala
public sealed record EmoteSetResult(EmoteSetStatus Status, string? Name, IReadOnlyList<RoomEmote> Emotes)
{
    public static EmoteSetResult NotFound { get; } = new(EmoteSetStatus.NotFound, null, []);
    public static EmoteSetResult Failed { get; } = new(EmoteSetStatus.Failed, null, []);
}
