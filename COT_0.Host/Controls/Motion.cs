using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace COT_0.Host.Controls;

// Storyboard decorativo que solo corre si Windows tiene las animaciones activas; se apaga al descargarse
public static class Motion
{
    public static readonly DependencyProperty StoryboardProperty = DependencyProperty.RegisterAttached(
        "Storyboard", typeof(Storyboard), typeof(Motion), new PropertyMetadata(null, OnStoryboardChanged));

    public static Storyboard? GetStoryboard(DependencyObject element) => (Storyboard?)element.GetValue(StoryboardProperty);

    public static void SetStoryboard(DependencyObject element, Storyboard? value) => element.SetValue(StoryboardProperty, value);

    public static bool IsEnabled => SystemParameters.ClientAreaAnimation;

    private static void OnStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element || e.OldValue is not null) return;
        PropertyChangedEventHandler onSystemChanged = (_, args) =>
        {
            if (args.PropertyName == nameof(SystemParameters.ClientAreaAnimation)) Update(element);
        };
        element.Loaded += (_, _) =>
        {
            SystemParameters.StaticPropertyChanged += onSystemChanged;
            Update(element);
        };
        element.Unloaded += (_, _) =>
        {
            SystemParameters.StaticPropertyChanged -= onSystemChanged;
            GetStoryboard(element)?.Stop(element);
        };
    }

    private static void Update(FrameworkElement element)
    {
        var storyboard = GetStoryboard(element);
        if (storyboard is null) return;
        if (IsEnabled) storyboard.Begin(element, true);
        else storyboard.Stop(element);
    }
}
