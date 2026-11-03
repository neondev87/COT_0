using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace COT_0.Host.Controls;

// Ceniza que sube lentamente (partículas grises con el mismo generador pseudoaleatorio del diseño)
public class AshLayer : Canvas
{
    public static readonly DependencyProperty CountProperty = Register(nameof(Count), 30);
    public static readonly DependencyProperty SeedProperty = Register(nameof(Seed), 11);
    public static readonly DependencyProperty MinOpacityProperty = Register(nameof(MinOpacity), 0.12);
    public static readonly DependencyProperty OpacityRangeProperty = Register(nameof(OpacityRange), 0.25);
    public static readonly DependencyProperty RiseProperty = Register(nameof(Rise), 780d);
    public static readonly DependencyProperty ParticleBrushProperty = Register(nameof(ParticleBrush), (Brush)Brushes.Gray);

    // Rangos del diseño: tamaño 1–3.4 px, duración 18–42 s, retraso 0–30 s, deriva -60–80 px
    private const double MinSize = 1, SizeRange = 2.4, MinSeconds = 18, SecondsRange = 24, DelayRange = 30;
    private const double MinDrift = -60, DriftRange = 140, FadeIn = 0.1, FadeOut = 0.9, BottomOffset = 20;

    private readonly List<(Ellipse Dot, double LeftRatio)> _dots = [];

    public AshLayer()
    {
        IsHitTestVisible = false;
        ClipToBounds = true;
        Loaded += (_, _) => Build();
        SizeChanged += (_, _) => Position();
    }

    public int Count { get => (int)GetValue(CountProperty); set => SetValue(CountProperty, value); }
    public int Seed { get => (int)GetValue(SeedProperty); set => SetValue(SeedProperty, value); }
    public double MinOpacity { get => (double)GetValue(MinOpacityProperty); set => SetValue(MinOpacityProperty, value); }
    public double OpacityRange { get => (double)GetValue(OpacityRangeProperty); set => SetValue(OpacityRangeProperty, value); }
    public double Rise { get => (double)GetValue(RiseProperty); set => SetValue(RiseProperty, value); }
    public Brush ParticleBrush { get => (Brush)GetValue(ParticleBrushProperty); set => SetValue(ParticleBrushProperty, value); }

    private static DependencyProperty Register<T>(string name, T defaultValue) =>
        DependencyProperty.Register(name, typeof(T), typeof(AshLayer), new PropertyMetadata(defaultValue));

    private void Build()
    {
        if (_dots.Count > 0) return;

        long seed = Seed;
        double Next()
        {
            seed = (seed * 9301 + 49297) % 233280;
            return seed / 233280d;
        }

        for (var i = 0; i < Count; i++)
        {
            var size = MinSize + Next() * SizeRange;
            var leftRatio = Next();
            var duration = TimeSpan.FromSeconds(MinSeconds + Next() * SecondsRange);
            var delay = TimeSpan.FromSeconds(Next() * DelayRange);
            var drift = Math.Round(MinDrift + Next() * DriftRange);
            var opacity = MinOpacity + Next() * OpacityRange;

            var dot = new Ellipse { Width = size, Height = size, Fill = ParticleBrush, Opacity = 0 };
            Children.Add(dot);
            _dots.Add((dot, leftRatio));
            Animate(dot, duration, delay, drift, opacity);
        }
        Position();
    }

    private void Animate(Ellipse dot, TimeSpan duration, TimeSpan delay, double drift, double opacity)
    {
        var move = new TranslateTransform();
        dot.RenderTransform = move;

        // Retraso negativo: cada partícula arranca a mitad de su recorrido
        var begin = -delay;
        move.BeginAnimation(TranslateTransform.XProperty, Linear(0, drift, duration, begin));
        move.BeginAnimation(TranslateTransform.YProperty, Linear(0, -Rise, duration, begin));

        var fade = new DoubleAnimationUsingKeyFrames { Duration = duration, BeginTime = begin, RepeatBehavior = RepeatBehavior.Forever };
        fade.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromPercent(0)));
        fade.KeyFrames.Add(new LinearDoubleKeyFrame(opacity, KeyTime.FromPercent(FadeIn)));
        fade.KeyFrames.Add(new LinearDoubleKeyFrame(opacity, KeyTime.FromPercent(FadeOut)));
        fade.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromPercent(1)));
        dot.BeginAnimation(OpacityProperty, fade);
    }

    private static DoubleAnimation Linear(double from, double to, TimeSpan duration, TimeSpan begin) =>
        new(from, to, duration) { BeginTime = begin, RepeatBehavior = RepeatBehavior.Forever };

    private void Position()
    {
        foreach (var (dot, leftRatio) in _dots)
        {
            SetLeft(dot, leftRatio * ActualWidth);
            SetTop(dot, ActualHeight + BottomOffset - dot.Height);
        }
    }
}
