namespace COT_0.Web.Services.Emotes;

// Emotes de la sala (Singleton): set de 7TV elegido por el host, emotes quitados y reportes.
// Queda en Web hasta que Core sume la interfaz (ver CONTRATO.md)
public interface IRoomEmotes
{
    string? SetId { get; }
    string? SetName { get; }
    EmoteSetStatus Status { get; }

    // Motivo del último fallo (None si cargó bien) y código HTTP si 7TV respondió con error
    EmoteSetError Error { get; }
    int? ErrorCode { get; }

    // Sube cada vez que cambia algo (para volver a pintar los mensajes)
    int Version { get; }

    IReadOnlyList<RoomEmote> Active { get; }
    IReadOnlyList<RoomEmote> Removed { get; }
    IReadOnlyList<EmoteReport> Reports { get; }

    // Cambió el set, la lista o los quitados
    event Action? Changed;
    event Action<EmoteReport>? Reported;

    // ID o URL del set; null o vacío apaga los emotes. Espera la primera carga
    Task<EmoteSetStatus> UseSetAsync(string? setIdOrUrl);

    // Solo emotes activos (los quitados se muestran como texto)
    RoomEmote? Find(string name);
    IReadOnlyList<RoomEmote> Search(string? query, int max);

    // false si el emote no está activo o ese invitado ya lo reportó
    bool Report(string emoteId, string userId, string userName);

    void Remove(string emoteId);
    void Restore(string emoteId);
    void ClearReports();
}
