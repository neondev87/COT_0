// Encabezado de muestra de las vistas de ejemplo: mismo dibujo que EncabezadoPantalla, sin regreso ni atajo
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Componentes;

/// <summary>Solo pinta ← INICIO | Title.</summary>
public sealed partial class EncabezadoEjemplo : UserControl
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(EncabezadoEjemplo), new PropertyMetadata(string.Empty));

    public EncabezadoEjemplo() => InitializeComponent();

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}
