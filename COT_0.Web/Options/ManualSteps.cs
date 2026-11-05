using COT_0.Web.Models;

namespace COT_0.Web.Options;

// Los cinco pasos del invitado que entra por QR (textos cortos, sin voseo)
public static class ManualSteps
{
    public static List<ManualStep> Default() =>
    [
        Step(ManualView.Join, "Entra y arma tu perfil", "Escanea el QR del host y elige cómo te verán.",
            ("Escanea el QR con tu celular y escribe tu nombre.", "name"),
            ("Sube una foto desde la cámara o tu galería.", "photo"),
            ("Elige la bandera de tu facultad y toca ENTRAR AL CHAT.", "faculty")),
        Step(ManualView.Chat, "El chat: texto, imágenes y notas de voz", "Aquí platicas con todos, en vivo.",
            ("Escribe tu mensaje y presiona ENTER para enviarlo.", "text"),
            ("Con el botón de imagen compartes fotos.", "attach"),
            ("Nota de voz: toca el micrófono para grabar y tócalo otra vez para enviarla.", "mic")),
        Step(ManualView.Emotes, "Emotes y reportes", "Los emotes se mueven, como en Kick y Twitch.",
            ("Toca la carita para abrir los emotes, o escribe : y su nombre.", "emotes"),
            ("¿Un emote ofensivo? Mantenlo presionado (clic derecho en PC) y toca REPORTAR.", "report")),
        Step(ManualView.Music, "Música", "La música ambienta la sala.",
            ("El host pone la música desde su computadora y suena en el salón.", null),
            ("¿Quieres otra canción? Pídesela al host en el chat.", null)),
        Step(ManualView.Exit, "Salir", "Para irte, solo cierra la pestaña.",
            ("Si vuelves a escanear el QR, entras con tu mismo perfil.", null),
            ("Cuando el host cierra la sala, todos salen y ves este aviso.", "closed")),
    ];

    private static ManualStep Step(ManualView view, string title, string text, params (string Text, string? Focus)[] notices) => new()
    {
        View = view,
        Title = title,
        Text = text,
        Notices = notices.Select(n => new ManualNotice { Text = n.Text, Focus = n.Focus }).ToList(),
    };
}
