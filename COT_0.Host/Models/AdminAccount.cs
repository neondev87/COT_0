// Un administrador tal como se guarda en admin.json (sin lógica)
namespace COT_0.Host.Models;

/// <summary>Nunca guarda la contraseña: solo su hash PBKDF2 + SHA-256 y su salt propio.</summary>
public class AdminAccount
{
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
