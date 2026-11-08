// Normaliza y revisa la respuesta secreta (al registrarla y al contestarla)
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Configuracion;

namespace COT_0.HostWinUI.Services.Validaciones;

/// <summary>"  Mango Verde " y "mangoverde" cuentan como la misma respuesta.</summary>
public static class SecurityAnswerValidator
{
    private static ValidationSettings Limits => AppConfig.Settings.Validation;

    /// <summary>Recorta, quita todos los espacios y pasa a minúsculas (invariant).</summary>
    public static string Normalize(string answer) =>
        new string(answer.Trim().Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();

    /// <summary>Al registrar: hace falta elegir pregunta y escribir respuesta.</summary>
    public static CredentialError Validate(string? questionKey, string answer) =>
        string.IsNullOrEmpty(questionKey) ? CredentialError.QuestionEmpty : ValidateAnswer(answer);

    /// <summary>La respuesta normalizada no puede quedar vacía ni pasar del límite.</summary>
    public static CredentialError ValidateAnswer(string answer)
    {
        var normalized = Normalize(answer);
        if (normalized.Length == 0) return CredentialError.AnswerEmpty;
        if (normalized.Length > Limits.MaxAnswerLength) return CredentialError.AnswerTooLong;
        return CredentialError.None;
    }
}
