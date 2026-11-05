using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace COT_0.Host.Controls;

// Recuadro con flecha del manual sobre el elemento Target de la vista de muestra.
// Va dentro de un Canvas hermano de la vista; Arrow es el lado donde se dibuja la flecha (siempre apunta al recuadro)
public class ManualHighlight : Control
{
    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.Register(nameof(Target), typeof(string), typeof(ManualHighlight), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ArrowProperty =
        DependencyProperty.Register(nameof(Arrow), typeof(Dock), typeof(ManualHighlight), new PropertyMetadata(Dock.Left));

    public static readonly DependencyProperty InflateProperty =
        DependencyProperty.Register(nameof(Inflate), typeof(double), typeof(ManualHighlight), new PropertyMetadata(0d));

    private FrameworkElement? _target;

    public ManualHighlight()
    {
        Loaded += (_, _) => LayoutUpdated += OnLayoutUpdated;
        Unloaded += (_, _) => LayoutUpdated -= OnLayoutUpdated;
    }

    // x:Name del elemento a resaltar (se busca en todo el escenario, aunque esté dentro de otro control)
    public string Target
    {
        get => (string)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public Dock Arrow
    {
        get => (Dock)GetValue(ArrowProperty);
        set => SetValue(ArrowProperty, value);
    }

    // Margen extra alrededor del elemento
    public double Inflate
    {
        get => (double)GetValue(InflateProperty);
        set => SetValue(InflateProperty, value);
    }

    // Sigue al elemento sin contar las animaciones de entrada (RenderTransform)
    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        if (VisualTreeHelper.GetParent(this) is not Canvas canvas || VisualTreeHelper.GetParent(canvas) is not Visual stage) return;
        _target ??= Find(stage, Target);
        if (_target is null || !_target.IsVisible) return;

        var origin = LayoutOffset(_target, stage);
        Place(Canvas.LeftProperty, origin.X - Inflate);
        Place(Canvas.TopProperty, origin.Y - Inflate);
        Place(WidthProperty, _target.ActualWidth + Inflate * 2);
        Place(HeightProperty, _target.ActualHeight + Inflate * 2);
    }

    private void Place(DependencyProperty property, double value)
    {
        if (!((double)GetValue(property)).Equals(value)) SetValue(property, value);
    }

    private static Point LayoutOffset(Visual element, Visual ancestor)
    {
        var offset = new Vector();
        for (var current = element; current is not null && current != ancestor; current = VisualTreeHelper.GetParent(current) as Visual)
            offset += VisualTreeHelper.GetOffset(current);
        return (Point)offset;
    }

    private static FrameworkElement? Find(DependencyObject root, string name)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is FrameworkElement element && element.Name == name) return element;
            if (Find(child, name) is { } found) return found;
        }
        return null;
    }
}
