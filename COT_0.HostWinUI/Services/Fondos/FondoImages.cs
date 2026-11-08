// Decodifica imágenes de fondo y cuadros de video (miniatura o cuadro fijo) a un tamaño chico, async
using System.Runtime.InteropServices;
using COT_0.HostWinUI.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace COT_0.HostWinUI.Services.Fondos;

/// <summary>
/// Reemplaza a ThumbnailImage y VideoThumbnail del WPF (que pausaban un MediaElement por video).
/// El cuadro de un video lo da Windows (la miniatura del explorador). Se llama desde el hilo de la UI;
/// regresa null si el archivo no se pudo leer.
/// </summary>
public class FondoImages
{
    public Task<BitmapImage?> LoadAsync(string path, FondoKind kind, uint size) =>
        kind == FondoKind.Image ? LoadImageAsync(path, size) : LoadVideoFrameAsync(path, size);

    private static async Task<BitmapImage?> LoadImageAsync(string path, uint size)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            using var stream = await file.OpenReadAsync();
            var image = new BitmapImage { DecodePixelWidth = (int)size };
            await image.SetSourceAsync(stream);
            return image;
        }
        catch (Exception ex) when (IsReadError(ex))
        {
            return null;
        }
    }

    private static async Task<BitmapImage?> LoadVideoFrameAsync(string path, uint size)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            using var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem, size, ThumbnailOptions.ResizeThumbnail);
            if (thumbnail is null || thumbnail.Type != ThumbnailType.Image) return null;
            var image = new BitmapImage();
            await image.SetSourceAsync(thumbnail);
            return image;
        }
        catch (Exception ex) when (IsReadError(ex))
        {
            return null;
        }
    }

    private static bool IsReadError(Exception ex) =>
        ex is IOException or UnauthorizedAccessException or ArgumentException or COMException;
}
