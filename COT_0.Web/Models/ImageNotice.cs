namespace COT_0.Web.Models;

// Avisos de imágenes para el invitado ({0} = megas permitidos)
public static class ImageNotice
{
    public const string TooLarge = "La imagen pesa más de {0} MB aunque se comprimió. Prueba con otra.";
    public const string Invalid = "El archivo no es una imagen válida (JPG, PNG, WebP o GIF).";
    public const string Unreadable = "Tu navegador no pudo abrir esa imagen. Prueba con otra (JPG o PNG).";
    public const string RoomFull = "La sala llegó al límite de imágenes. No se pueden mandar más.";
    public const string SendFailed = "No se pudo enviar la imagen. Intenta de nuevo.";
    public const string NotInRoom = "Ya no estás en la sala.";

    public static string ForSize(long maxBytes) => string.Format(TooLarge, Math.Round(maxBytes / 1024d / 1024d, 1));
}
