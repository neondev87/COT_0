namespace COT_0.Web.Models;

// Perfil del invitado guardado en el navegador, ligado al código de la sala
public record StoredGuest(string RoomCode, string UserId, string Name, string? PhotoUrl, string? FacultyFlag);
