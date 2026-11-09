// Regla única de todos los campos del Host: solo letras y números (ñ y acentos sí), sin espacios, arrobas ni símbolos
namespace COT_0.Host.Services.Validaciones;

/// <summary>La usan el filtro de escritura (FiltroCampo), los validadores y los servicios antes de guardar.</summary>
public static class InputRules
{
    public static bool IsAllowed(char c) => char.IsLetterOrDigit(c);

    /// <summary>true si todo el texto cumple la regla (vacío también cumple; "vacío" lo revisa cada validador).</summary>
    public static bool IsAllowed(string? text) => text is null || text.All(IsAllowed);

    /// <summary>Quita lo que no cumple (tecla o texto pegado).</summary>
    public static string Clean(string text) => IsAllowed(text) ? text : new string(text.Where(IsAllowed).ToArray());
}
