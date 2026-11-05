using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace COT_0.Host.Controls;

// Estática de TV sin señal: unos pocos cuadros de ruido generados una vez y rotados (barato para la CPU)
public class NoiseLayer : Image
{
    private const int Size = 256;
    private const int FrameCount = 6;
    private const double FramesPerSecond = 12;

    private readonly DispatcherTimer _timer = new(DispatcherPriority.Background) { Interval = TimeSpan.FromSeconds(1 / FramesPerSecond) };
    private BitmapSource[] _frames = [];
    private int _index;

    public NoiseLayer()
    {
        Stretch = Stretch.Fill;
        IsHitTestVisible = false;
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        _timer.Tick += (_, _) => Source = _frames[_index = (_index + 1) % _frames.Length];
        Loaded += (_, _) => Start();
        Unloaded += (_, _) => _timer.Stop();
    }

    private void Start()
    {
        if (_frames.Length == 0) _frames = Enumerable.Range(0, FrameCount).Select(seed => BuildFrame(new Random(seed))).ToArray();
        Source = _frames[0];
        if (!SystemParameters.ClientAreaAnimation) return;
        _timer.Start();
    }

    private static BitmapSource BuildFrame(Random random)
    {
        var pixels = new byte[Size * Size];
        random.NextBytes(pixels);
        var frame = BitmapSource.Create(Size, Size, 96, 96, PixelFormats.Gray8, null, pixels, Size);
        frame.Freeze();
        return frame;
    }
}
