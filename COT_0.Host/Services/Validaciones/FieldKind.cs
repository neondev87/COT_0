// Tipo de campo para el filtro al escribir (FiltroCampo)
namespace COT_0.Host.Services.Validaciones;

/// <summary>Cada tipo tiene su regla en InputRules.Clean.</summary>
public enum FieldKind
{
    /// <summary>Letras y números (respuesta secreta, buscador).</summary>
    Text,

    /// <summary>Solo letras; la primera en mayúscula y el resto en minúscula.</summary>
    UserName,

    /// <summary>Letras, números y los símbolos de Inputs:PasswordSymbols; nunca espacios.</summary>
    Password,
}
