namespace COT_0.Core.Services;

/// <summary>Estado de la sala: la abre el host y la ven los invitados</summary>
public interface IRoomService
{
    // Se dispara al abrir o cerrar la sala
    event Action? Changed;

    // Código corto visible ("7F3K"); null si está cerrada
    string? Code { get; }

    // UTC; null si está cerrada
    DateTime? OpenedAt { get; }

    bool IsOpen { get; }

    // Genera un código nuevo y marca la hora de apertura
    void Open();

    // Cierra la sala y vacía el chat
    Task CloseAsync();
}
