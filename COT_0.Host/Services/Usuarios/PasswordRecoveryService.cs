// "¿Olvidaste tu contraseña?": lee la pregunta secreta del usuario indicado y cambia su contraseña
using COT_0.Host.Models;
using COT_0.Host.Services.Validaciones;

namespace COT_0.Host.Services.Usuarios;

/// <summary>La respuesta se normaliza igual que al registrarla antes de compararla.</summary>
public static class PasswordRecoveryService
{
    /// <summary>Clave de la pregunta de ese usuario; null si no existe o no tiene pregunta (la vista no distingue).</summary>
    public static async Task<string?> GetQuestionKeyAsync(string userName)
    {
        var user = HostAccounts.Find(await HostAccounts.LoadAsync(), userName);
        return user is not null && HostAccounts.HasSecurityQuestion(user) ? user.SecurityQuestion : null;
    }

    public static async Task<bool> VerifyAnswerAsync(string userName, string answer)
    {
        var user = HostAccounts.Find(await HostAccounts.LoadAsync(), userName);
        return user is not null && Matches(user, answer);
    }

    /// <summary>Vuelve a revisar la respuesta y guarda la contraseña nueva; false si no coincide.</summary>
    public static async Task<bool> ResetPasswordAsync(string userName, string answer, string newPassword)
    {
        var users = await HostAccounts.LoadAsync();
        var user = HostAccounts.Find(users, userName);
        if (user is null || !Matches(user, answer)) return false;

        HostAccounts.SetPassword(user, newPassword);
        await HostAccounts.SaveAsync(users);
        return true;
    }

    private static bool Matches(HostUser user, string answer) =>
        HostAccounts.HasSecurityQuestion(user)
        && SecretHasher.Verify(SecurityAnswerValidator.Normalize(answer), user.AnswerHash, user.AnswerSalt);
}
