// Botón del manual: tecla dibujada opcional (keycap) + texto; estilos en Views/Estilos/Manual.xaml
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

/// <summary>
/// Es un Button: el texto va en Content y la acción en Command. Con KeyText es un atajo
/// con la tecla dibujada; sin KeyText la tecla se oculta y queda el botón redondeado.
/// </summary>
public sealed class BotonManual : Button
{
    private const string KeyPartName = "Key";

    public static readonly DependencyProperty KeyTextProperty =
        DependencyProperty.Register(nameof(KeyText), typeof(string), typeof(BotonManual), new PropertyMetadata(string.Empty));

    public string KeyText
    {
        get => (string)GetValue(KeyTextProperty);
        set => SetValue(KeyTextProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (GetTemplateChild(KeyPartName) is UIElement key)
            key.Visibility = string.IsNullOrEmpty(KeyText) ? Visibility.Collapsed : Visibility.Visible;
    }
}
