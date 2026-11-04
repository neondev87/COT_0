namespace COT_0.Web.Models;

// Avisos para el invitado; los códigos de error los manda voice.js
public static class VoiceNotice
{
    public const string TooShort = "La nota de voz es muy corta. Toca el micrófono para grabar y otra vez para enviar.";
    public const string TooLarge = "La nota de voz es demasiado grande.";
    public const string Invalid = "El archivo no es un audio válido.";
    public const string SendFailed = "No se pudo enviar la nota de voz. Intenta de nuevo.";

    public static string ForError(string code) => code switch
    {
        "denied" => "No diste permiso para usar el micrófono. Actívalo en los ajustes del navegador para enviar notas de voz.",
        "nodevice" => "No se encontró ningún micrófono.",
        "busy" => "Otra app está usando el micrófono.",
        "insecure" => "El micrófono solo funciona con HTTPS: entra desde el enlace del QR.",
        "unsupported" => "Tu navegador no puede grabar audio.",
        _ => "No se pudo usar el micrófono.",
    };
}
