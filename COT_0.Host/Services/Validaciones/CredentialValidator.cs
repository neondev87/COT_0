// Revisa usuario y contraseña antes del login, del registro o del cambio de contraseña
using COT_0.Host.Models;

namespace COT_0.Host.Services.Validaciones;

/// <summary>Solo letras y números (InputRules), sin espacios ni símbolos, y hasta Inputs:MaxCaracteres.</summary>
public static class CredentialValidator
{
    /// <summary>Te regresa el primer error que encuentre, o None si todo está bien.</summary>
    public static CredentialError Validate(string userName, string password)
    {
        var error = ValidateUserName(userName);
        return error != CredentialError.None ? error : ValidatePassword(password);
    }

    /// <summary>Registro: usuario válido y la contraseña escrita igual las dos veces.</summary>
    public static CredentialError ValidateRegistration(string userName, string password, string confirmation)
    {
        var error = ValidateUserName(userName);
        return error != CredentialError.None ? error : ValidateNewPassword(password, confirmation);
    }

    public static CredentialError ValidateUserName(string userName)
    {
        if (string.IsNullOrEmpty(userName)) return CredentialError.UserEmpty;
        if (!InputRules.IsAllowed(userName)) return CredentialError.UserInvalid;
        if (InputLimits.IsTooLong(userName)) return CredentialError.UserTooLong;
        return CredentialError.None;
    }

    public static CredentialError ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return CredentialError.PasswordEmpty;
        if (!InputRules.IsAllowed(password)) return CredentialError.PasswordInvalid;
        if (InputLimits.IsTooLong(password)) return CredentialError.PasswordTooLong;
        return CredentialError.None;
    }

    /// <summary>Contraseña nueva: mismas reglas y escrita igual las dos veces.</summary>
    public static CredentialError ValidateNewPassword(string password, string confirmation)
    {
        var error = ValidatePassword(password);
        if (error != CredentialError.None) return error;
        return password == confirmation ? CredentialError.None : CredentialError.PasswordMismatch;
    }
}
