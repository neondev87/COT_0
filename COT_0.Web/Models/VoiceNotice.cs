namespace COT_0.Web.Models;

// Avisos para el invitado; los códigos de error los manda voice.js
public static class VoiceNotice
{
    public const string TooShort = "Mantené presionado el micrófono para grabar.";
    public const string TooLarge = "La nota de voz es demasiado grande.";
    public const string Invalid = "El archivo no es un audio válido.";
    public const string SendFailed = "No se pudo enviar la nota de voz. Probá de nuevo.";

    public static string ForError(string code) => code switch
    {
        "denied" => "Bloqueaste el micrófono. Habilitalo en los ajustes del navegador para mandar notas de voz.",
        "nodevice" => "No se encontró ningún micrófono.",
        "busy" => "Otra app está usando el micrófono.",
        "insecure" => "El micrófono solo funciona con HTTPS: entrá desde el link del QR.",
        "unsupported" => "Tu navegador no puede grabar audio.",
        _ => "No se pudo usar el micrófono.",
    };
}
