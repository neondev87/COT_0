namespace COT_0.Web.Models;

// Formato real de la imagen subida, según sus magic bytes
public enum ImageFormat
{
    Unknown,
    Jpeg,
    Png,
    WebP,
    Gif,
}

// Lo que dicen los primeros bytes de la imagen; Width/Height en 0 si no se pudieron leer
public readonly record struct ImageInfo(ImageFormat Format, int Width, int Height)
{
    public static readonly ImageInfo Unknown = new(ImageFormat.Unknown, 0, 0);

    public string ContentType => Format switch
    {
        ImageFormat.Jpeg => "image/jpeg",
        ImageFormat.Png => "image/png",
        ImageFormat.WebP => "image/webp",
        ImageFormat.Gif => "image/gif",
        _ => "application/octet-stream",
    };
}

// Estado del adjunto de imagen del invitado
public enum ImageUploadState
{
    Idle,
    Preparing,
    Preview,
    Uploading,
}

// Imagen guardada en memoria
public sealed record ImageClip(string Id, byte[] Bytes, string ContentType, int Width, int Height);

// Lo que necesita la burbuja: URL y tamaño de la miniatura ya calculado (reserva el lugar antes de cargar)
public sealed record ImageView(string Url, int ThumbWidth, int ThumbHeight, bool Available);
