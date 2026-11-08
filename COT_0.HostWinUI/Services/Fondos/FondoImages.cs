// Decodifica imágenes de fondo y el cuadro fijo de los videos (miniatura o fondo) al tamaño pedido, async
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Registro;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace COT_0.HostWinUI.Services.Fondos;

/// <summary>
/// Reemplaza a ThumbnailImage y VideoThumbnail del WPF (que pausaban un MediaElement por video).
/// El cuadro de un video sale de FondoFrameCache (un JPEG en caché). Se llama desde el hilo de la UI;
/// regresa null si no se pudo leer (el motivo queda en el registro de medios).
/// </summary>
public class FondoImages(FondoFrameCache frames)
{
    public async Task<BitmapImage?> LoadAsync(string path, FondoKind kind, uint width)
    {
        var source = kind == FondoKind.Image ? path : await frames.GetAsync(path);
        return source is null ? null : await LoadImageAsync(source, width);
    }

    /// <summary>Decodifica a lo ancho pedido (nunca más que el original) con la escala de alta calidad de WinUI.</summary>
    public static async Task<BitmapImage?> LoadImageAsync(string path, uint width)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            using var stream = await file.OpenReadAsync();
            var image = new BitmapImage { DecodePixelWidth = (int)width, DecodePixelType = DecodePixelType.Physical };
            await image.SetSourceAsync(stream);
            return image;
        }
        catch (Exception ex) when (FondoFailures.IsMediaException(ex))
        {
            await MediaLog.WriteAsync("imagen", path, ex);
            return null;
        }
    }
}
