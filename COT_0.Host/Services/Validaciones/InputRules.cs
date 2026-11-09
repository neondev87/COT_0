// Reglas de escritura de los campos del Host: texto (letras y números), usuario (solo letras, tipo Erick) y contraseña (más símbolos)
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Validaciones;

/// <summary>La usan el filtro de escritura (FiltroCampo), los validadores y los servicios antes de guardar.</summary>
public static class InputRules
{
    /// <summary>Símbolos que acepta la contraseña (Inputs:PasswordSymbols).</summary>
    public static string PasswordSymbols => AppConfig.Settings.Inputs.PasswordSymbols;

    public static bool IsAllowed(char c) => char.IsLetterOrDigit(c);

    /// <summary>true si todo el texto cumple la regla (vacío también cumple; "vacío" lo revisa cada validador).</summary>
    public static bool IsAllowed(string? text) => text is null || text.All(IsAllowed);

    public static bool IsPasswordChar(char c) => char.IsLetterOrDigit(c) || IsPasswordSymbol(c);

    public static bool IsPasswordSymbol(char c) => PasswordSymbols.Contains(c);

    /// <summary>Usuario: solo letras, la primera en mayúscula y el resto en minúscula.</summary>
    public static bool IsUserName(string? text) =>
        !string.IsNullOrEmpty(text) && text.All(char.IsLetter) && text == NormalizeUserName(text);

    /// <summary>Quita lo que no sea letra y deja la primera en mayúscula y el resto en minúscula.</summary>
    public static string NormalizeUserName(string text)
    {
        var letters = new string(text.Where(char.IsLetter).ToArray());
        return letters.Length == 0 ? letters : char.ToUpperInvariant(letters[0]) + letters[1..].ToLowerInvariant();
    }

    /// <summary>Deja solo lo que cumple la regla del tipo de campo (tecla o texto pegado).</summary>
    public static string Clean(string text, FieldKind kind) => kind switch
    {
        FieldKind.UserName => NormalizeUserName(text),
        FieldKind.Password => new string(text.Where(IsPasswordChar).ToArray()),
        _ => new string(text.Where(IsAllowed).ToArray()),
    };

    /// <summary>true si el texto ya está limpio para ese tipo de campo.</summary>
    public static bool IsClean(string text, FieldKind kind) => Clean(text, kind) == text;
}
