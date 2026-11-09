// Convierte la foto de perfil guardada en una imagen para la vista (tarjetas de USUARIOS y firma)
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace COT_0.Host.Services.Perfiles;

/// <summary>
/// Se decodifica desde los bytes (no desde la ruta): así una foto nueva con el mismo nombre de archivo
/// no sale de la caché de imágenes. Se llama desde el hilo de la UI; null si no hay foto o no se pudo leer.
/// </summary>
public static class ProfileImages
{
    public static async Task<BitmapImage?> LoadAsync(string userName)
    {
        try
        {
            if (await ProfilePhotos.LoadAsync(userName) is not { } bytes) return null;

            using var stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(bytes.AsBuffer());
            stream.Seek(0);
            var image = new BitmapImage();
            await image.SetSourceAsync(stream);
            return image;
        }
        catch (Exception ex) when (ProfilePhotos.IsPhotoError(ex))
        {
            return null;
        }
    }
}
