// Un administrador tal como se guarda en admin.json (sin lógica)
namespace COT_0.Host.Models;

/// <summary>
/// Nunca guarda la contraseña ni la respuesta secreta: solo su hash PBKDF2 + SHA-256 y su salt propio.
/// La pregunta se configura la primera vez que entra (vacía en admins creados antes de que existiera).
/// </summary>
public class AdminAccount
{
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    /// <summary>Clave del texto de la pregunta (ej. "Text.Question.Fruit"), no el texto.</summary>
    public string SecurityQuestion { get; set; } = string.Empty;
    public string AnswerHash { get; set; } = string.Empty;
    public string AnswerSalt { get; set; } = string.Empty;
}
