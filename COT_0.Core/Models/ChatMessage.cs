namespace COT_0.Core.Models;

// Mensaje del chat grupal
public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = string.Empty;

    // Copia de los datos del autor al momento de enviar
    public string UserName { get; set; } = string.Empty;
    public string? UserPhotoUrl { get; set; }
    public string? UserFacultyFlag { get; set; }

    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
