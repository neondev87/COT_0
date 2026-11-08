// Un usuario del login tal como se guarda en usuarios.json (sin lógica)
namespace COT_0.Host.Models;

/// <summary>
/// Nunca guarda la contraseña ni la respuesta secreta: solo su hash y su salt.
/// La pregunta se configura después de entrar por primera vez (SecurityQuestion vacío hasta entonces).
/// </summary>
public class HostUser
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
