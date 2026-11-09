// Recorta una imagen al cuadrado del centro y la reduce a Profiles:PhotoMaxPixels como PNG, con el decodificador de Windows
using COT_0.Host.Services.Configuracion;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace COT_0.Host.Services.Perfiles;

/// <summary>Nunca agranda: una foto chica queda de su tamaño. Regresa los bytes del PNG listos para guardar.</summary>
public static class PhotoCropper
{
    public static async Task<byte[]> CropAsync(IRandomAccessStream source)
    {
        var decoder = await BitmapDecoder.CreateAsync(source);
        var side = Math.Min(decoder.PixelWidth, decoder.PixelHeight);
        var scale = Math.Min(1.0, (double)AppConfig.Settings.Profiles.PhotoMaxPixels / side);
        var target = (uint)Math.Max(1, Math.Round(side * scale));

        // Bounds se aplica después de escalar: se escala toda la imagen y se corta el cuadrado del centro
        var width = (uint)Math.Max(target, Math.Round(decoder.PixelWidth * scale));
        var height = (uint)Math.Max(target, Math.Round(decoder.PixelHeight * scale));
        var transform = new BitmapTransform
        {
            ScaledWidth = width,
            ScaledHeight = height,
            InterpolationMode = BitmapInterpolationMode.Fant,
            Bounds = new BitmapBounds { X = (width - target) / 2, Y = (height - target) / 2, Width = target, Height = target },
        };
        var pixels = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform,
            ExifOrientationMode.RespectExifOrientation, ColorManagementMode.ColorManageToSRgb);

        using var output = new InMemoryRandomAccessStream();
        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, output);
        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, target, target, 96, 96, pixels.DetachPixelData());
        await encoder.FlushAsync();

        var bytes = new byte[output.Size];
        using var reader = new DataReader(output.GetInputStreamAt(0));
        await reader.LoadAsync((uint)output.Size);
        reader.ReadBytes(bytes);
        return bytes;
    }
}
