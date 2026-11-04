using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Decodifica portadas chicas y congeladas (usable fuera del hilo de la UI);
// si la imagen no es cuadrada la recorta a cuadrado centrado
public class CoverDecoder(IConfiguration configuration)
{
    public ImageSource? Decode(string file)
    {
        try
        {
            var size = configuration.GetValue<int>("Audio:CoverDecodeSize");
            var (width, height) = ReadPixelSize(file);
            var image = Load(file, size, width >= height);
            if (width == height) return image;

            var side = Math.Min(image.PixelWidth, image.PixelHeight);
            var area = new Int32Rect((image.PixelWidth - side) / 2, (image.PixelHeight - side) / 2, side, side);
            var cropped = new CroppedBitmap(image, area);
            cropped.Freeze();
            return cropped;
        }
        catch (Exception ex) when (ex is NotSupportedException or IOException or FileFormatException or ArgumentException)
        {
            return null;
        }
    }

    // Lee solo el encabezado para saber el tamaño sin decodificar la imagen entera
    private static (int Width, int Height) ReadPixelSize(string file)
    {
        using var stream = File.OpenRead(file);
        var frame = BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None).Frames[0];
        return (frame.PixelWidth, frame.PixelHeight);
    }

    // El lado corto queda en "size"; el largo se ajusta manteniendo la proporción
    private static BitmapImage Load(string file, int size, bool landscape)
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        if (landscape) image.DecodePixelHeight = size;
        else image.DecodePixelWidth = size;
        image.UriSource = new Uri(file);
        image.EndInit();
        image.Freeze();
        return image;
    }
}
