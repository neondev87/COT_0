using System.Windows;
using System.Windows.Media.Animation;

namespace COT_0.Host.Controls;

// Muestra un elemento del manual solo entre dos fases del paso (From ≤ Phase ≤ To), con fundido.
// Phase se hereda: se pone una vez en la raíz del paso
public static class ManualCue
{
    private static readonly Duration FadeDuration = TimeSpan.FromSeconds(0.3);

    public static readonly DependencyProperty PhaseProperty = DependencyProperty.RegisterAttached(
        "Phase", typeof(int), typeof(ManualCue), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits, OnChanged));

    public static readonly DependencyProperty FromProperty = DependencyProperty.RegisterAttached(
        "From", typeof(int), typeof(ManualCue), new PropertyMetadata(-1, OnChanged));

    public static readonly DependencyProperty ToProperty = DependencyProperty.RegisterAttached(
        "To", typeof(int), typeof(ManualCue), new PropertyMetadata(int.MaxValue, OnChanged));

    // Estado ya aplicado, para no repetir la animación
    private static readonly DependencyProperty IsShownProperty = DependencyProperty.RegisterAttached(
        "IsShown", typeof(bool?), typeof(ManualCue), new PropertyMetadata(null));

    public static int GetPhase(DependencyObject d) => (int)d.GetValue(PhaseProperty);
    public static void SetPhase(DependencyObject d, int value) => d.SetValue(PhaseProperty, value);
    public static int GetFrom(DependencyObject d) => (int)d.GetValue(FromProperty);
    public static void SetFrom(DependencyObject d, int value) => d.SetValue(FromProperty, value);
    public static int GetTo(DependencyObject d) => (int)d.GetValue(ToProperty);
    public static void SetTo(DependencyObject d, int value) => d.SetValue(ToProperty, value);

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element || GetFrom(element) < 0) return;

        var phase = GetPhase(element);
        var show = GetFrom(element) <= phase && phase <= GetTo(element);
        var previous = (bool?)element.GetValue(IsShownProperty);
        if (previous == show) return;

        element.SetValue(IsShownProperty, show);
        element.IsHitTestVisible = show;
        if (previous is null) element.Opacity = show ? 1 : 0;
        else element.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(show ? 1 : 0, FadeDuration));
    }
}
