using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace COT_0.Host.Controls;

// Estática de TV sin señal: un bitmap chico que se repinta con ruido en cada cuadro; solo corre mientras se ve
public class NoiseLayer : Image
{
    private const int Size = 256;
    private const double FramesPerSecond = 12;

    private readonly DispatcherTimer _timer = new(DispatcherPriority.Render) { Interval = TimeSpan.FromSeconds(1 / FramesPerSecond) };
    private readonly WriteableBitmap _bitmap = new(Size, Size, 96, 96, PixelFormats.Gray8, null);
    private readonly byte[] _pixels = new byte[Size * Size];
    private readonly Int32Rect _area = new(0, 0, Size, Size);

    public NoiseLayer()
    {
        Source = _bitmap;
        Stretch = Stretch.Fill;
        IsHitTestVisible = false;
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        Paint();
        _timer.Tick += (_, _) => Paint();
        IsVisibleChanged += (_, _) => UpdateTimer();
        Unloaded += (_, _) => _timer.Stop();
    }

    private void UpdateTimer()
    {
        if (IsVisible) _timer.Start();
        else _timer.Stop();
    }

    private void Paint()
    {
        Random.Shared.NextBytes(_pixels);
        _bitmap.WritePixels(_area, _pixels, Size, 0);
    }
}
