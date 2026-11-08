namespace COT_0.Web.Models;

/// <summary>Perfil del invitado guardado en el navegador, ligado al código de la sala</summary>
public record StoredGuest(string RoomCode, string UserId, string Name, string? PhotoUrl, string? FacultyFlag);
