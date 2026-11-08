// Normaliza y revisa la respuesta secreta (al registrarla y al contestarla)
using COT_0.Host.Models;

namespace COT_0.Host.Services.Validaciones;

/// <summary>"  Mango Verde " y "mangoverde" cuentan como la misma respuesta.</summary>
public static class SecurityAnswerValidator
{
    /// <summary>Recorta, quita todos los espacios y pasa a minúsculas (invariant).</summary>
    public static string Normalize(string answer) =>
        new string(answer.Trim().Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();

    /// <summary>Al registrar: hace falta elegir pregunta y escribir respuesta.</summary>
    public static CredentialError Validate(string? questionKey, string answer) =>
        string.IsNullOrEmpty(questionKey) ? CredentialError.QuestionEmpty : ValidateAnswer(answer);

    /// <summary>La respuesta normalizada no puede quedar vacía, y lo escrito no puede pasar de Inputs:MaxCaracteres.</summary>
    public static CredentialError ValidateAnswer(string answer)
    {
        if (Normalize(answer).Length == 0) return CredentialError.AnswerEmpty;
        if (InputLimits.IsTooLong(answer)) return CredentialError.AnswerTooLong;
        return CredentialError.None;
    }
}
