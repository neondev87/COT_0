using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace COT_0.Host.Controls;

// Tarjeta inclinada del menú del dashboard (estilo en Themes/MenuCard.xaml)
public class MenuCard : Button
{
    public static readonly DependencyProperty NumberProperty =
        DependencyProperty.Register(nameof(Number), typeof(string), typeof(MenuCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(MenuCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(MenuCard), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(Geometry), typeof(MenuCard), new PropertyMetadata(null));

    public string Number
    {
        get => (string)GetValue(NumberProperty);
        set => SetValue(NumberProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public Geometry? Icon
    {
        get => (Geometry?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
