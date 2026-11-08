// Genera el QR de la sala con QRCoder y lo entrega como imagen de WinUI
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using COT_0.HostWinUI.Services.Configuracion;
using Microsoft.UI.Xaml.Media.Imaging;
using QRCoder;
using Windows.Storage.Streams;

namespace COT_0.HostWinUI.Services.Sala;

/// <summary>
/// Igual que el del WPF (corrección H para tapar el centro con el rombo "C0"), pero la
/// BitmapImage es la de Microsoft.UI.Xaml y se carga async. Llamar desde el hilo de la UI.
/// </summary>
public class QrService
{
    public async Task<BitmapImage> CreateAsync(string text)
    {
        var settings = AppConfig.Settings.Qr;
        var png = await Task.Run(() => GeneratePng(text, settings));

        using var stream = new InMemoryRandomAccessStream();
        await stream.WriteAsync(png.AsBuffer());
        stream.Seek(0);

        var image = new BitmapImage();
        await image.SetSourceAsync(stream);
        return image;
    }

    private static byte[] GeneratePng(string text, QrSettings settings)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.H);
        using var qrCode = new PngByteQRCode(data);
        return qrCode.GetGraphic(settings.PixelsPerModule, ToRgba(settings.DarkColor), ToRgba(settings.LightColor), drawQuietZones: false);
    }

    // "#RRGGBB" o "#AARRGGBB" → RGBA
    private static byte[] ToRgba(string hex)
    {
        var value = uint.Parse(hex.TrimStart('#'), NumberStyles.HexNumber);
        var alpha = hex.Length > 7 ? (byte)(value >> 24) : (byte)255;
        return [(byte)(value >> 16), (byte)(value >> 8), (byte)value, alpha];
    }
}
