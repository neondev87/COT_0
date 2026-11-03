using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Configuration;
using QRCoder;

namespace COT_0.Host.Services;

// Genera el QR (QRCoder, corrección H para tapar el centro con el rombo)
public class QrService(IConfiguration configuration)
{
    public BitmapImage Generate(string text)
    {
        var pixelsPerModule = configuration.GetValue("Qr:PixelsPerModule", 20);
        var dark = ToRgba(configuration["Qr:DarkColor"] ?? "#000000");
        var light = ToRgba(configuration["Qr:LightColor"] ?? "#FFFFFF");

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.H);
        using var qrCode = new PngByteQRCode(data);
        var png = qrCode.GetGraphic(pixelsPerModule, dark, light, drawQuietZones: false);

        using var stream = new MemoryStream(png);
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();
        image.Freeze();
        return image;
    }

    // "#RRGGBB" o "#AARRGGBB" → RGBA
    private static byte[] ToRgba(string hex)
    {
        var value = uint.Parse(hex.TrimStart('#'), NumberStyles.HexNumber);
        var hasAlpha = hex.Length > 7;
        var alpha = hasAlpha ? (byte)(value >> 24) : (byte)255;
        return [(byte)(value >> 16), (byte)(value >> 8), (byte)value, alpha];
    }
}
