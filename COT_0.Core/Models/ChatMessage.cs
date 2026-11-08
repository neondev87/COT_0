namespace COT_0.Core.Models;

/// <summary>Mensaje del chat grupal</summary>
public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = string.Empty;

    // Copia de los datos del autor al momento de enviar
    public string UserName { get; set; } = string.Empty;
    public string? UserPhotoUrl { get; set; }
    public string? UserFacultyFlag { get; set; }

    public MessageKind Kind { get; set; } = MessageKind.Text;

    public string Text { get; set; } = string.Empty;

    // Solo en notas de voz (Kind = Voice) e imágenes (Kind = Image): ruta relativa del archivo
    public string? AudioUrl { get; set; }
    public string? ImageUrl { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
