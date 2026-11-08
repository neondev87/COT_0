// Registro e inicio de sesión del host (una sola cuenta), con contraseña y respuesta hasheadas
using COT_0.HostWinUI.Models;

namespace COT_0.HostWinUI.Services.Usuarios;

/// <summary>Cuenta del host guardada en usuarios.json.</summary>
public static class UserService
{
    /// <summary>true si ya hay un host registrado.</summary>
    public static async Task<bool> HasUsersAsync() => HostAccounts.Host(await HostAccounts.LoadAsync()) is not null;

    /// <summary>Crea la cuenta con su pregunta secreta; false si ya hay un host.</summary>
    public static async Task<bool> RegisterAsync(string userName, string password, string questionKey, string answer)
    {
        var users = await HostAccounts.LoadAsync();
        if (HostAccounts.Host(users) is not null) return false;

        var user = new HostUser { UserName = userName, CreatedAt = DateTime.UtcNow };
        HostAccounts.SetPassword(user, password);
        HostAccounts.SetSecurityQuestion(user, questionKey, answer);
        users.Add(user);
        await HostAccounts.SaveAsync(users);
        return true;
    }

    /// <summary>true si el usuario existe y la contraseña coincide con su hash.</summary>
    public static async Task<bool> LoginAsync(string userName, string password)
    {
        var user = HostAccounts.Find(await HostAccounts.LoadAsync(), userName);
        return user is not null && SecretHasher.Verify(password, user.PasswordHash, user.Salt);
    }

    /// <summary>true si la cuenta es de antes de la recuperación y todavía no tiene pregunta.</summary>
    public static async Task<bool> NeedsSecurityQuestionAsync(string userName)
    {
        var user = HostAccounts.Find(await HostAccounts.LoadAsync(), userName);
        return user is not null && !HostAccounts.HasSecurityQuestion(user);
    }

    /// <summary>Configura la pregunta de una cuenta vieja (después de ENTRAR).</summary>
    public static async Task SetSecurityQuestionAsync(string userName, string questionKey, string answer)
    {
        var users = await HostAccounts.LoadAsync();
        var user = HostAccounts.Find(users, userName)
            ?? throw new InvalidOperationException("La cuenta del host ya no existe.");

        HostAccounts.SetSecurityQuestion(user, questionKey, answer);
        await HostAccounts.SaveAsync(users);
    }
}
