// Revisa usuario y contraseña antes de intentar el login o el registro
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Configuracion;

namespace COT_0.HostWinUI.Services.Validaciones;

/// <summary>Solo letras y números: sin espacios ni símbolos.</summary>
public static class CredentialValidator
{
    private static ValidationSettings Limits => AppConfig.Settings.Validation;

    /// <summary>Te regresa el primer error que encuentre, o None si todo está bien.</summary>
    public static CredentialError Validate(string userName, string password)
    {
        if (string.IsNullOrEmpty(userName)) return CredentialError.UserEmpty;
        if (!IsAlphanumeric(userName)) return CredentialError.UserInvalid;
        if (userName.Length > Limits.MaxUserLength) return CredentialError.UserTooLong;

        if (string.IsNullOrEmpty(password)) return CredentialError.PasswordEmpty;
        if (!IsAlphanumeric(password)) return CredentialError.PasswordInvalid;
        if (password.Length > Limits.MaxPasswordLength) return CredentialError.PasswordTooLong;

        return CredentialError.None;
    }

    private static bool IsAlphanumeric(string text) => text.All(char.IsLetterOrDigit);
}
