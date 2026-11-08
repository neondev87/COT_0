// Encabezado con regreso al dashboard (clic en INICIO o Esc) y el título de la pantalla
using COT_0.HostWinUI.Services.Ventana;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Componentes;

public sealed partial class EncabezadoPantalla : UserControl
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(EncabezadoPantalla), new PropertyMetadata(string.Empty));

    public EncabezadoPantalla() => InitializeComponent();

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    private void OnBackClick(object sender, RoutedEventArgs e) => Navigator.ShowDashboard();
}
