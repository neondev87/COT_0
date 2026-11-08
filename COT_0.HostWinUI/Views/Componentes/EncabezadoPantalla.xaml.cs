// Encabezado con regreso (clic o Esc) a la pantalla anterior, sala o dashboard, y el título de la pantalla
using COT_0.HostWinUI.Services.Textos;
using COT_0.HostWinUI.Services.Ventana;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Componentes;

public sealed partial class EncabezadoPantalla : UserControl
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(EncabezadoPantalla), new PropertyMetadata(string.Empty));

    public EncabezadoPantalla()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // En Loaded la navegación ya terminó y el BackStack dice de dónde viniste
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var text = UiText.Get(Navigator.CameFromRoom ? "Text.Header.BackToRoom" : "Text.Header.Back");
        BackLabel.Text = text;
        AutomationProperties.SetName(BackButton, text);
    }

    private void OnBackClick(object sender, RoutedEventArgs e) => Navigator.GoBack();
}
