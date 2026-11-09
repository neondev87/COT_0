// Revisa la contraseña contra todas sus reglas y te dice cuáles fallan
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Validaciones;

/// <summary>Mínimo Inputs:MinPasswordCaracteres, máximo Inputs:MaxCaracteres, un número, un símbolo y sin espacios.</summary>
public static class PasswordValidator
{
    public static int MinCharacters => AppConfig.Settings.Inputs.MinPasswordCaracteres;

    /// <summary>Todas las reglas que no cumple (None si está bien).</summary>
    public static PasswordProblems Check(string? password)
    {
        var text = password ?? string.Empty;
        var problems = PasswordProblems.None;
        if (text.Length < MinCharacters) problems |= PasswordProblems.TooShort;
        if (InputLimits.IsTooLong(text)) problems |= PasswordProblems.TooLong;
        if (!text.Any(char.IsDigit)) problems |= PasswordProblems.NoDigit;
        if (!text.Any(InputRules.IsPasswordSymbol)) problems |= PasswordProblems.NoSymbol;
        if (text.Any(char.IsWhiteSpace)) problems |= PasswordProblems.HasSpaces;
        if (text.Any(c => !char.IsWhiteSpace(c) && !InputRules.IsPasswordChar(c))) problems |= PasswordProblems.InvalidChars;
        return problems;
    }

    /// <summary>Las banderas encendidas, en orden, para armar el mensaje.</summary>
    public static IEnumerable<PasswordProblems> List(PasswordProblems problems) =>
        Enum.GetValues<PasswordProblems>().Where(p => p != PasswordProblems.None && problems.HasFlag(p));
}
