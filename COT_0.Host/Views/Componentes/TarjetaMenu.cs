// Tarjeta inclinada del menú del dashboard: número, título, subtítulo e icono (estilo en Views/Estilos/TarjetaMenu.xaml)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace COT_0.Host.Views.Componentes;

/// <summary>Es un Button: el título va en Content y la acción en Command.</summary>
public sealed class TarjetaMenu : Button
{
    private const string IconPartName = "Icon";

    public static readonly DependencyProperty NumberProperty =
        DependencyProperty.Register(nameof(Number), typeof(string), typeof(TarjetaMenu), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(TarjetaMenu), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(TarjetaMenu), new PropertyMetadata(string.Empty));

    public string Number
    {
        get => (string)GetValue(NumberProperty);
        set => SetValue(NumberProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    /// <summary>Datos del trazo del icono (24×24), como el Data de un Path.</summary>
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    // Cada tarjeta necesita su propia Geometry: WinUI no deja compartir una entre dos Path
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (GetTemplateChild(IconPartName) is Path icon && !string.IsNullOrEmpty(Icon))
            icon.Data = (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), Icon);
    }
}
