using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace COT_0.Host.Controls;

// Estática de TV: grano fino del tamaño del control (sin cuadros), teñido con Tint y repintado en cada cuadro.
// Solo corre mientras se ve; con las animaciones de Windows apagadas queda quieta
public class NoiseLayer : FrameworkElement
{
    private const int MaxSide = 1200;

    private readonly DispatcherTimer _timer = new(DispatcherPriority.Render);
    private WriteableBitmap? _bitmap;
    private byte[] _pixels = [];

    public NoiseLayer()
    {
        IsHitTestVisible = false;
        RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Linear);
        _timer.Tick += (_, _) => Paint();
        IsVisibleChanged += (_, _) => UpdateTimer();
        Unloaded += (_, _) => _timer.Stop();
    }

    // Color del grano: cada punto toma una intensidad al azar de transparente a este color
    public Color Tint { get; set; } = Colors.White;

    // Píxeles de pantalla por punto de ruido (2 = grano fino y suave)
    public double Grain { get; set; } = 2;

    public double FramesPerSecond { get; set; } = 24;

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        Rebuild();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        if (_bitmap is not null) drawingContext.DrawImage(_bitmap, new Rect(RenderSize));
    }

    private void Rebuild()
    {
        var width = (int)Math.Clamp(Math.Ceiling(RenderSize.Width / Grain), 1, MaxSide);
        var height = (int)Math.Clamp(Math.Ceiling(RenderSize.Height / Grain), 1, MaxSide);
        if (_bitmap?.PixelWidth == width && _bitmap.PixelHeight == height) return;
        _bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Indexed8, CreatePalette());
        _pixels = new byte[width * height];
        Paint();
        InvalidateVisual();
        UpdateTimer();
    }

    // Más puntos tenues que brillantes, como la nieve de una tele sin señal
    private BitmapPalette CreatePalette()
    {
        var colors = Enumerable.Range(0, 256)
            .Select(i => Color.FromArgb((byte)(255 * Math.Pow(i / 255.0, 2)), Tint.R, Tint.G, Tint.B))
            .ToList();
        return new BitmapPalette(colors);
    }

    private void UpdateTimer()
    {
        _timer.Interval = TimeSpan.FromSeconds(1 / FramesPerSecond);
        if (IsVisible && Motion.IsEnabled && _bitmap is not null) _timer.Start();
        else _timer.Stop();
    }

    private void Paint()
    {
        if (_bitmap is null) return;
        Random.Shared.NextBytes(_pixels);
        _bitmap.WritePixels(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight), _pixels, _bitmap.PixelWidth, 0);
    }
}
