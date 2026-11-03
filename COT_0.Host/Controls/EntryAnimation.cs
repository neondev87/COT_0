using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace COT_0.Host.Controls;

// Animación de entrada al cargar: aparece (opacity) y se desliza desde OffsetX/OffsetY, con retraso
public static class EntryAnimation
{
    public static readonly DependencyProperty DurationProperty = DependencyProperty.RegisterAttached(
        "Duration", typeof(Duration), typeof(EntryAnimation), new PropertyMetadata(Duration.Automatic, OnDurationChanged));

    public static readonly DependencyProperty DelayProperty = DependencyProperty.RegisterAttached(
        "Delay", typeof(TimeSpan), typeof(EntryAnimation), new PropertyMetadata(TimeSpan.Zero));

    public static readonly DependencyProperty OffsetXProperty = DependencyProperty.RegisterAttached(
        "OffsetX", typeof(double), typeof(EntryAnimation), new PropertyMetadata(0d));

    public static readonly DependencyProperty OffsetYProperty = DependencyProperty.RegisterAttached(
        "OffsetY", typeof(double), typeof(EntryAnimation), new PropertyMetadata(0d));

    public static readonly DependencyProperty EaseProperty = DependencyProperty.RegisterAttached(
        "Ease", typeof(KeySpline), typeof(EntryAnimation), new PropertyMetadata(null));

    public static Duration GetDuration(DependencyObject d) => (Duration)d.GetValue(DurationProperty);
    public static void SetDuration(DependencyObject d, Duration value) => d.SetValue(DurationProperty, value);
    public static TimeSpan GetDelay(DependencyObject d) => (TimeSpan)d.GetValue(DelayProperty);
    public static void SetDelay(DependencyObject d, TimeSpan value) => d.SetValue(DelayProperty, value);
    public static double GetOffsetX(DependencyObject d) => (double)d.GetValue(OffsetXProperty);
    public static void SetOffsetX(DependencyObject d, double value) => d.SetValue(OffsetXProperty, value);
    public static double GetOffsetY(DependencyObject d) => (double)d.GetValue(OffsetYProperty);
    public static void SetOffsetY(DependencyObject d, double value) => d.SetValue(OffsetYProperty, value);
    public static KeySpline? GetEase(DependencyObject d) => (KeySpline?)d.GetValue(EaseProperty);
    public static void SetEase(DependencyObject d, KeySpline? value) => d.SetValue(EaseProperty, value);

    private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element) return;
        element.Loaded -= OnLoaded;
        element.Loaded += OnLoaded;
    }

    private static void OnLoaded(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var translate = new TranslateTransform();
        element.RenderTransform = translate;

        element.BeginAnimation(UIElement.OpacityProperty, Build(element, 0, 1));
        translate.BeginAnimation(TranslateTransform.XProperty, Build(element, GetOffsetX(element), 0));
        translate.BeginAnimation(TranslateTransform.YProperty, Build(element, GetOffsetY(element), 0));
    }

    // El primer keyframe se aplica ya durante el retraso (como animation-fill-mode: both)
    private static DoubleAnimationUsingKeyFrames Build(FrameworkElement element, double from, double to)
    {
        var delay = GetDelay(element);
        var duration = GetDuration(element).TimeSpan;
        var animation = new DoubleAnimationUsingKeyFrames { Duration = delay + duration };
        animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(from, KeyTime.FromTimeSpan(TimeSpan.Zero)));
        animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(from, KeyTime.FromTimeSpan(delay)));
        animation.KeyFrames.Add(new SplineDoubleKeyFrame(to, KeyTime.FromTimeSpan(delay + duration), GetEase(element) ?? new KeySpline()));
        return animation;
    }
}
