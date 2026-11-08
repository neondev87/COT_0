namespace COT_0.Web.Models;

/// <summary>Lo que dicen los primeros bytes de la imagen; Width/Height en 0 si no se pudieron leer</summary>
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
