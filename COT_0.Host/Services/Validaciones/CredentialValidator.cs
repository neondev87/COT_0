// Revisa usuario y contraseña antes del login, del registro o del cambio de contraseña
using COT_0.Host.Models;

namespace COT_0.Host.Services.Validaciones;

/// <summary>Usuario solo letras (InputRules.IsUserName); contraseña con PasswordValidator; todo hasta Inputs:MaxCaracteres.</summary>
public static class CredentialValidator
{
    /// <summary>Registro: usuario válido y contraseña que cumple todas las reglas.</summary>
    public static CredentialError Validate(string userName, string password)
    {
        var error = ValidateUserName(userName);
        return error != CredentialError.None ? error : ValidatePassword(password);
    }

    /// <summary>ENTRAR: aquí no se exigen las reglas nuevas de contraseña, así se detecta una cuenta vieja.</summary>
    public static CredentialError ValidateSignIn(string userName, string password)
    {
        var error = ValidateUserName(userName);
        if (error != CredentialError.None) return error;
        if (string.IsNullOrEmpty(password)) return CredentialError.PasswordEmpty;
        return InputLimits.IsTooLong(password) ? CredentialError.PasswordTooLong : CredentialError.None;
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
        if (InputLimits.IsTooLong(userName)) return CredentialError.UserTooLong;
        return InputRules.IsUserName(userName) ? CredentialError.None : CredentialError.UserInvalid;
    }

    /// <summary>PasswordRules si falla alguna regla; el detalle lo da PasswordValidator.Check.</summary>
    public static CredentialError ValidatePassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return CredentialError.PasswordEmpty;
        return PasswordValidator.Check(password) == PasswordProblems.None ? CredentialError.None : CredentialError.PasswordRules;
    }

    /// <summary>Contraseña nueva: mismas reglas y escrita igual las dos veces.</summary>
    public static CredentialError ValidateNewPassword(string password, string confirmation)
    {
        var error = ValidatePassword(password);
        if (error != CredentialError.None) return error;
        return password == confirmation ? CredentialError.None : CredentialError.PasswordMismatch;
    }
}
