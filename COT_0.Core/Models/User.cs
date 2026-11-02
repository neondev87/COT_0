namespace COT_0.Core.Models;

// Invitado conectado al chat (vive solo en memoria)
public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = string.Empty;

    // Foto de perfil como URL o data URI (base64)
    public string? PhotoUrl { get; set; }

    // Clave de la bandera de facultad, ej: "ingenieria" -> wwwroot/img/flags/ingenieria.png
    public string? FacultyFlag { get; set; }

    // Conexión SignalR actual del usuario
    public string? ConnectionId { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
