// Decodifica la portada grande de MÚSICA a su tamaño real de despliegue, con tope según sus píxeles
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Registro;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace COT_0.Host.Services.Musica;

/// <summary>
/// Antes la portada grande reusaba la de la fila (decodificada a Audio:CoverDecodeSize = 96 px) estirada a 300:
/// por eso se veía pixelada. Ahora se lee el tamaño real del archivo y se muestra a lo más en Audio:CoverLargeSize
/// y nunca a más de Audio:CoverMaxUpscale veces su tamaño (una portada chica sale más chica, no pixelada).
/// Al reducir, el decodificador de Windows escala con interpolación de alta calidad (DecodePixelWidth).
/// </summary>
public class CoverImages
{
    public async Task<CoverArt?> LoadAsync(string path)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            using var stream = await file.OpenReadAsync();
            var decoder = await BitmapDecoder.CreateAsync(stream);
            var (width, height) = Fit(decoder.OrientedPixelWidth, decoder.OrientedPixelHeight);

            stream.Seek(0);
            var image = new BitmapImage { DecodePixelType = DecodePixelType.Logical };
            // Solo se pide decodificar más chico; si hay que ampliar, se decodifica completo y el Image lo escala
            if (width < decoder.OrientedPixelWidth) image.DecodePixelWidth = (int)width;
            await image.SetSourceAsync(stream);
            return new CoverArt(image, width, height);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException
                                       or System.Runtime.InteropServices.COMException)
        {
            await MediaLog.WriteAsync("portada", path, ex);
            return null;
        }
    }

    // Cabe en el recuadro sin deformarse y sin ampliarse más del tope
    private static (double Width, double Height) Fit(uint pixelWidth, uint pixelHeight)
    {
        var audio = AppConfig.Settings.Audio;
        var scale = Math.Min(Math.Min(audio.CoverLargeSize / pixelWidth, audio.CoverLargeSize / pixelHeight), audio.CoverMaxUpscale);
        return (Math.Round(pixelWidth * scale), Math.Round(pixelHeight * scale));
    }
}
