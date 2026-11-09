// Registro (hasta Usuarios:Maximo) e inicio de sesión de los usuarios del host, con contraseña y respuesta hasheadas
using COT_0.Host.Models;
using COT_0.Host.Services.Validaciones;

namespace COT_0.Host.Services.Usuarios;

/// <summary>Usuarios guardados en usuarios.json; vuelve a validar todo lo que llega (no confía en la pantalla).</summary>
public static class UserService
{
    /// <summary>true si ya hay al menos un usuario registrado.</summary>
    public static async Task<bool> HasUsersAsync() => (await HostAccounts.LoadAsync()).Count > 0;

    /// <summary>true si ya se llegó a Usuarios:Maximo.</summary>
    public static async Task<bool> IsFullAsync() => (await HostAccounts.LoadAsync()).Count >= HostAccounts.Maximum;

    /// <summary>
    /// Crea el usuario sin pregunta secreta (se pide al entrar). Una cuenta vieja con el mismo nombre se reemplaza
    /// (así se recrea con las reglas nuevas). El error de validación, UsersFull o UserTaken si no se puede.
    /// </summary>
    public static async Task<CredentialError> RegisterAsync(string userName, string password)
    {
        var error = CredentialValidator.Validate(userName, password);
        if (error != CredentialError.None) return error;

        var users = await HostAccounts.LoadAsync();
        var existing = HostAccounts.Find(users, userName);
        if (existing is not null && HostAccounts.IsCurrent(existing)) return CredentialError.UserTaken;
        if (existing is null && users.Count >= HostAccounts.Maximum) return CredentialError.UsersFull;

        var user = new HostUser { UserName = userName, CreatedAt = DateTime.UtcNow };
        HostAccounts.SetPassword(user, password);
        if (existing is null) users.Add(user);
        else users[users.IndexOf(existing)] = user;
        await HostAccounts.SaveAsync(users);
        return CredentialError.None;
    }

    /// <summary>None si entra; WrongCredentials si no coincide; AccountOutdated si coincide pero la cuenta no cumple las reglas nuevas.</summary>
    public static async Task<CredentialError> LoginAsync(string userName, string password)
    {
        var error = CredentialValidator.ValidateSignIn(userName, password);
        if (error != CredentialError.None) return error;

        var user = HostAccounts.Find(await HostAccounts.LoadAsync(), userName);
        if (user is null || !SecretHasher.Verify(password, user.PasswordHash, user.Salt)) return CredentialError.WrongCredentials;
        return HostAccounts.IsCurrent(user) && CredentialValidator.ValidatePassword(password) == CredentialError.None
            ? CredentialError.None
            : CredentialError.AccountOutdated;
    }

    /// <summary>true si ese usuario todavía no configura su pregunta secreta.</summary>
    public static async Task<bool> NeedsSecurityQuestionAsync(string userName)
    {
        var user = HostAccounts.Find(await HostAccounts.LoadAsync(), userName);
        return user is not null && !HostAccounts.HasSecurityQuestion(user);
    }

    /// <summary>Guarda la pregunta de ese usuario (después de ENTRAR por primera vez); si no es válida, no guarda y te dice por qué.</summary>
    public static async Task<CredentialError> SetSecurityQuestionAsync(string userName, string questionKey, string answer)
    {
        var error = SecurityAnswerValidator.Validate(questionKey, answer);
        if (error != CredentialError.None) return error;

        var users = await HostAccounts.LoadAsync();
        var user = HostAccounts.Find(users, userName)
            ?? throw new InvalidOperationException("El usuario ya no existe.");

        HostAccounts.SetSecurityQuestion(user, questionKey, answer);
        await HostAccounts.SaveAsync(users);
        return CredentialError.None;
    }
}
