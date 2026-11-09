// Normaliza y revisa la respuesta secreta (al registrarla y al contestarla)
using COT_0.Host.Models;

namespace COT_0.Host.Services.Validaciones;

/// <summary>
/// Al escribir solo se aceptan letras y números (InputRules); Normalize sigue quitando espacios y mayúsculas
/// para que "MangoVerde" coincida con respuestas guardadas antes como "Mango Verde".
/// </summary>
public static class SecurityAnswerValidator
{
    /// <summary>Recorta, quita todos los espacios y pasa a minúsculas (invariant).</summary>
    public static string Normalize(string answer) =>
        new string(answer.Trim().Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();

    /// <summary>Al registrar: hace falta elegir pregunta y escribir respuesta.</summary>
    public static CredentialError Validate(string? questionKey, string answer) =>
        string.IsNullOrEmpty(questionKey) ? CredentialError.QuestionEmpty : ValidateAnswer(answer);

    /// <summary>No vacía, solo letras y números y hasta Inputs:MaxCaracteres.</summary>
    public static CredentialError ValidateAnswer(string answer)
    {
        if (string.IsNullOrEmpty(answer)) return CredentialError.AnswerEmpty;
        if (!InputRules.IsAllowed(answer)) return CredentialError.AnswerInvalid;
        if (InputLimits.IsTooLong(answer)) return CredentialError.AnswerTooLong;
        return CredentialError.None;
    }
}
