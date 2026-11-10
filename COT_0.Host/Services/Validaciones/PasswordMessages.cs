// Arma el mensaje de la contraseña con todas las reglas que fallan, una por línea ("Falta un número", "No uses espacios")
using System.Globalization;
using COT_0.Host.Services.Textos;

namespace COT_0.Host.Services.Validaciones;

/// <summary>Cada regla tiene su texto "Text.Password.{regla}"; {0} es el mínimo, el máximo o la lista de símbolos.</summary>
public static class PasswordMessages
{
    public static string Describe(PasswordProblems problems) =>
        string.Join(Environment.NewLine, PasswordValidator.List(problems).Select(DescribeOne));

    private static string DescribeOne(PasswordProblems problem)
    {
        object value = problem switch
        {
            PasswordProblems.TooShort => PasswordValidator.MinCharacters,
            PasswordProblems.TooLong => InputLimits.MaxCharacters,
            _ => string.Join(' ', InputRules.PasswordSymbols.ToCharArray()),
        };
        return string.Format(CultureInfo.CurrentCulture, UiText.Get($"Text.Password.{problem}"), value);
    }
}
