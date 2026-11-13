// Hashea y revisa la contraseña y la respuesta secreta de un admin (PBKDF2 + SHA-256 con SecretHasher)
using COT_0.Host.Models;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Validaciones;

namespace COT_0.Host.Services.Administradores;

/// <summary>La respuesta se compara normalizada: en minúsculas y sin espacios (SecurityAnswerValidator.Normalize).</summary>
public static class AdminSecrets
{
    /// <summary>Genera una contraseña nueva, guarda solo su hash y la regresa para mostrarla una vez.</summary>
    public static string SetNewPassword(AdminAccount admin)
    {
        var password = PasswordGenerator.Create();
        var hashed = SecretHasher.Create(password);
        admin.PasswordHash = hashed.Hash;
        admin.Salt = hashed.Salt;
        return password;
    }

    public static bool VerifyPassword(AdminAccount admin, string password) =>
        !string.IsNullOrEmpty(password) && !InputLimits.IsTooLong(password)
        && SecretHasher.Verify(password, admin.PasswordHash, admin.Salt);

    public static bool HasQuestion(AdminAccount admin) =>
        !string.IsNullOrEmpty(admin.SecurityQuestion) && !string.IsNullOrEmpty(admin.AnswerHash) && !string.IsNullOrEmpty(admin.AnswerSalt);

    public static void SetQuestion(AdminAccount admin, string questionKey, string answer)
    {
        var hashed = SecretHasher.Create(SecurityAnswerValidator.Normalize(answer));
        admin.SecurityQuestion = questionKey;
        admin.AnswerHash = hashed.Hash;
        admin.AnswerSalt = hashed.Salt;
    }

    public static bool VerifyAnswer(AdminAccount admin, string answer)
    {
        var normalized = SecurityAnswerValidator.Normalize(answer);
        return HasQuestion(admin) && SecurityAnswerValidator.ValidateAnswer(normalized) == CredentialError.None
            && SecretHasher.Verify(normalized, admin.AnswerHash, admin.AnswerSalt);
    }
}
