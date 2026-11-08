// Registro (hasta Usuarios:Maximo) e inicio de sesión de los usuarios del host, con contraseña y respuesta hasheadas
using COT_0.Host.Models;

namespace COT_0.Host.Services.Usuarios;

/// <summary>Usuarios guardados en usuarios.json; lo de adentro de la app no sabe cuál entró.</summary>
public static class UserService
{
    /// <summary>true si ya hay al menos un usuario registrado.</summary>
    public static async Task<bool> HasUsersAsync() => (await HostAccounts.LoadAsync()).Count > 0;

    /// <summary>true si ya se llegó a Usuarios:Maximo.</summary>
    public static async Task<bool> IsFullAsync() => (await HostAccounts.LoadAsync()).Count >= HostAccounts.Maximum;

    /// <summary>Crea el usuario sin pregunta secreta (se pide al entrar). UsersFull o UserTaken si no se puede.</summary>
    public static async Task<CredentialError> RegisterAsync(string userName, string password)
    {
        var users = await HostAccounts.LoadAsync();
        if (users.Count >= HostAccounts.Maximum) return CredentialError.UsersFull;
        if (HostAccounts.Find(users, userName) is not null) return CredentialError.UserTaken;

        var user = new HostUser { UserName = userName, CreatedAt = DateTime.UtcNow };
        HostAccounts.SetPassword(user, password);
        users.Add(user);
        await HostAccounts.SaveAsync(users);
        return CredentialError.None;
    }

    /// <summary>true si el usuario existe y la contraseña coincide con su hash.</summary>
    public static async Task<bool> LoginAsync(string userName, string password)
    {
        var user = HostAccounts.Find(await HostAccounts.LoadAsync(), userName);
        return user is not null && SecretHasher.Verify(password, user.PasswordHash, user.Salt);
    }

    /// <summary>true si ese usuario todavía no configura su pregunta secreta.</summary>
    public static async Task<bool> NeedsSecurityQuestionAsync(string userName)
    {
        var user = HostAccounts.Find(await HostAccounts.LoadAsync(), userName);
        return user is not null && !HostAccounts.HasSecurityQuestion(user);
    }

    /// <summary>Guarda la pregunta de ese usuario (después de ENTRAR por primera vez).</summary>
    public static async Task SetSecurityQuestionAsync(string userName, string questionKey, string answer)
    {
        var users = await HostAccounts.LoadAsync();
        var user = HostAccounts.Find(users, userName)
            ?? throw new InvalidOperationException("El usuario ya no existe.");

        HostAccounts.SetSecurityQuestion(user, questionKey, answer);
        await HostAccounts.SaveAsync(users);
    }
}
