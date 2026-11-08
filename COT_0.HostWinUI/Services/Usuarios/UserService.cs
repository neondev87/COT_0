// Registro e inicio de sesión del host, con contraseña hasheada (PBKDF2 + salt)
using System.Security.Cryptography;
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Almacenamiento;
using COT_0.HostWinUI.Services.Configuracion;

namespace COT_0.HostWinUI.Services.Usuarios;

/// <summary>Cuentas del host guardadas en usuarios.json.</summary>
public static class UserService
{
    private static string FileName => AppConfig.Settings.Storage.UsersFile;

    private static SecuritySettings Security => AppConfig.Settings.Security;

    /// <summary>true si ya hay al menos un host registrado.</summary>
    public static async Task<bool> HasUsersAsync() => (await LoadAsync()).Count > 0;

    /// <summary>Crea la cuenta; regresa false si ese usuario ya existe.</summary>
    public static async Task<bool> RegisterAsync(string userName, string password)
    {
        var users = await LoadAsync();
        if (Find(users, userName) is not null) return false;

        var salt = RandomNumberGenerator.GetBytes(Security.SaltBytes);
        users.Add(new HostUser
        {
            UserName = userName,
            Salt = Convert.ToBase64String(salt),
            PasswordHash = Convert.ToBase64String(Hash(password, salt)),
            CreatedAt = DateTime.UtcNow,
        });
        await JsonStore.SaveAsync(FileName, users);
        return true;
    }

    /// <summary>true si el usuario existe y la contraseña coincide con su hash.</summary>
    public static async Task<bool> LoginAsync(string userName, string password)
    {
        var user = Find(await LoadAsync(), userName);
        if (user is null) return false;

        var expected = Convert.FromBase64String(user.PasswordHash);
        var actual = Hash(password, Convert.FromBase64String(user.Salt));
        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }

    private static Task<List<HostUser>> LoadAsync() => JsonStore.LoadAsync<List<HostUser>>(FileName);

    private static HostUser? Find(List<HostUser> users, string userName) =>
        users.FirstOrDefault(u => string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase));

    private static byte[] Hash(string password, byte[] salt) =>
        Rfc2898DeriveBytes.Pbkdf2(password, salt, Security.HashIterations, HashAlgorithmName.SHA256, Security.HashBytes);
}
