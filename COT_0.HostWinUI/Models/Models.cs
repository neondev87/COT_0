// Clases simples que se guardan en JSON (sin lógica)
namespace COT_0.HostWinUI.Models;

/// <summary>Cuenta del host. Nunca guarda la contraseña: solo su hash y su salt.</summary>
public class HostUser
{
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>Una entrada de un invitado a la sala.</summary>
public class Visit
{
    public string Name { get; set; } = string.Empty;
    public DateTime EnteredAt { get; set; }
}

/// <summary>Por qué se rechazó lo que escribiste en el login.</summary>
public enum CredentialError
{
    None,
    UserEmpty,
    UserInvalid,
    UserTooLong,
    PasswordEmpty,
    PasswordInvalid,
    PasswordTooLong,
}

/// <summary>Textos de un diálogo de confirmación: mensaje, botón de confirmar y de cancelar.</summary>
public record ConfirmTexts(string Message, string Confirm, string Cancel);
