using System.Windows;
using System.Windows.Controls;

namespace COT_0.Host.Controls;

// Barra de ventana: título opcional y botones minimizar / maximizar / cerrar (estilo en Themes/Window.xaml)
public class CaptionBar : Control
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(CaptionBar), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ShowMaximizeProperty =
        DependencyProperty.Register(nameof(ShowMaximize), typeof(bool), typeof(CaptionBar), new PropertyMetadata(true));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool ShowMaximize
    {
        get => (bool)GetValue(ShowMaximizeProperty);
        set => SetValue(ShowMaximizeProperty, value);
    }
}
