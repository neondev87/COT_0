// Genera la contraseña de un admin al azar con RandomNumberGenerator (criptográficamente segura)
using System.Security.Cryptography;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Validaciones;

namespace COT_0.Host.Services.Administradores;

/// <summary>
/// Admin:PasswordLength caracteres con al menos una mayúscula, una minúscula, un número y un símbolo
/// (Inputs:PasswordSymbols); sin espacios ni caracteres ambiguos. Siempre cumple PasswordValidator.
/// </summary>
public static class PasswordGenerator
{
    private static AdminSettings Admin => AppConfig.Settings.Admin;

    public static string Create()
    {
        string[] groups = [Admin.PasswordUpper, Admin.PasswordLower, Admin.PasswordDigits, InputRules.PasswordSymbols];
        var all = string.Concat(groups);
        var length = Math.Max(Admin.PasswordLength, groups.Length);

        // Uno de cada grupo y el resto de cualquiera; luego se revuelve para que no queden en orden fijo
        var chars = groups.Select(Pick).Concat(Enumerable.Range(0, length - groups.Length).Select(_ => Pick(all))).ToArray();
        RandomNumberGenerator.Shuffle(chars.AsSpan());
        var password = new string(chars);

        return PasswordValidator.Check(password) == PasswordProblems.None
            ? password
            : throw new InvalidOperationException("Admin:PasswordLength no cabe en las reglas de contraseña (Inputs).");
    }

    private static char Pick(string alphabet) => alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
}
