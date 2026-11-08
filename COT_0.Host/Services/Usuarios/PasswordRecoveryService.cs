// "¿Olvidaste tu contraseña?": lee la pregunta secreta del host y cambia la contraseña
using COT_0.Host.Models;
using COT_0.Host.Services.Validaciones;

namespace COT_0.Host.Services.Usuarios;

/// <summary>La respuesta se normaliza igual que al registrarla antes de compararla.</summary>
public static class PasswordRecoveryService
{
    /// <summary>Clave de la pregunta del host; null si no hay cuenta o si no tiene pregunta.</summary>
    public static async Task<string?> GetQuestionKeyAsync()
    {
        var user = HostAccounts.Host(await HostAccounts.LoadAsync());
        return user is not null && HostAccounts.HasSecurityQuestion(user) ? user.SecurityQuestion : null;
    }

    /// <summary>Usuario del host, para dejarlo escrito al regresar al login.</summary>
    public static async Task<string> GetUserNameAsync() =>
        HostAccounts.Host(await HostAccounts.LoadAsync())?.UserName ?? string.Empty;

    public static async Task<bool> VerifyAnswerAsync(string answer)
    {
        var user = HostAccounts.Host(await HostAccounts.LoadAsync());
        return user is not null && Matches(user, answer);
    }

    /// <summary>Vuelve a revisar la respuesta y guarda la contraseña nueva; false si no coincide.</summary>
    public static async Task<bool> ResetPasswordAsync(string answer, string newPassword)
    {
        var users = await HostAccounts.LoadAsync();
        var user = HostAccounts.Host(users);
        if (user is null || !Matches(user, answer)) return false;

        HostAccounts.SetPassword(user, newPassword);
        await HostAccounts.SaveAsync(users);
        return true;
    }

    private static bool Matches(HostUser user, string answer) =>
        HostAccounts.HasSecurityQuestion(user)
        && SecretHasher.Verify(SecurityAnswerValidator.Normalize(answer), user.AnswerHash, user.AnswerSalt);
}
