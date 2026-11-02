using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Configuration;
using QRCoder;

namespace COT_0.Host.Services;

// Genera el QR (QRCoder) con la URL pública de /join
public class QrService(IConfiguration configuration)
{
    public BitmapImage Generate(string text)
    {
        var pixelsPerModule = configuration.GetValue("Qr:PixelsPerModule", 20);

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(data);
        var png = qrCode.GetGraphic(pixelsPerModule);

        using var stream = new MemoryStream(png);
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();
        image.Freeze();
        return image;
    }
}
