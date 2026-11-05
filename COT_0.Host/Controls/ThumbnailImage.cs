using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace COT_0.Host.Controls;

// Imagen de miniatura decodificada al tamaño en pantalla (no a resolución completa) y solo mientras se ve.
// DecodeSide: el lado corto de la imagen queda de ese tamaño, así cubre la miniatura con UniformToFill
public class ThumbnailImage : Image
{
    public static readonly DependencyProperty FileProperty =
        DependencyProperty.Register(nameof(File), typeof(Uri), typeof(ThumbnailImage),
            new PropertyMetadata(null, (d, _) => ((ThumbnailImage)d).Update()));

    public static readonly DependencyProperty DecodeSideProperty =
        DependencyProperty.Register(nameof(DecodeSide), typeof(double), typeof(ThumbnailImage));

    public Uri? File
    {
        get => (Uri?)GetValue(FileProperty);
        set => SetValue(FileProperty, value);
    }

    public double DecodeSide
    {
        get => (double)GetValue(DecodeSideProperty);
        set => SetValue(DecodeSideProperty, value);
    }

    public ThumbnailImage() => IsVisibleChanged += (_, _) => Update();

    private void Update() => Source = IsVisible ? Decode() : null;

    private ImageSource? Decode()
    {
        if (File is null) return null;
        try
        {
            var side = (int)Math.Ceiling(DecodeSide * VisualTreeHelper.GetDpi(this).DpiScaleX);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            if (IsLandscape(File)) image.DecodePixelHeight = side;
            else image.DecodePixelWidth = side;
            image.UriSource = File;
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch (Exception ex) when (ex is NotSupportedException or IOException or FileFormatException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    // Solo lee el encabezado, sin decodificar la imagen entera
    private static bool IsLandscape(Uri file)
    {
        using var stream = System.IO.File.OpenRead(file.LocalPath);
        var frame = BitmapDecoder.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None).Frames[0];
        return frame.PixelWidth >= frame.PixelHeight;
    }
}
