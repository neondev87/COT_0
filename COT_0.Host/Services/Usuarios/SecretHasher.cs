// Hashea y compara secretos (contraseña y respuesta secreta) con PBKDF2 + SHA-256 y salt propio
using System.Security.Cryptography;
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Usuarios;

/// <summary>Un solo lugar para el hash: así contraseña y respuesta se guardan igual.</summary>
public static class SecretHasher
{
    private static SecuritySettings Security => AppConfig.Settings.Security;

    /// <summary>Genera un salt nuevo y te regresa hash y salt en Base64.</summary>
    public static HashedSecret Create(string secret)
    {
        var salt = RandomNumberGenerator.GetBytes(Security.SaltBytes);
        return new HashedSecret(Convert.ToBase64String(Hash(secret, salt)), Convert.ToBase64String(salt));
    }

    /// <summary>true si el secreto da el mismo hash (comparación en tiempo constante).</summary>
    public static bool Verify(string secret, string hash, string salt)
    {
        if (string.IsNullOrEmpty(hash) || string.IsNullOrEmpty(salt)) return false;

        var expected = Convert.FromBase64String(hash);
        var actual = Hash(secret, Convert.FromBase64String(salt));
        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }

    private static byte[] Hash(string secret, byte[] salt) =>
        Rfc2898DeriveBytes.Pbkdf2(secret, salt, Security.HashIterations, HashAlgorithmName.SHA256, Security.HashBytes);
}
