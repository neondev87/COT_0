// Encabezado con regreso (clic o Esc) a la pantalla anterior, sala o dashboard, y el título de la pantalla
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Ventana;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

public sealed partial class EncabezadoPantalla : UserControl
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(EncabezadoPantalla), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty HandlesEscapeProperty = DependencyProperty.Register(
        nameof(HandlesEscape), typeof(bool), typeof(EncabezadoPantalla), new PropertyMetadata(true));

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

    /// <summary>false en el manual: ahí Esc abre la despedida, no regresa.</summary>
    public bool HandlesEscape
    {
        get => (bool)GetValue(HandlesEscapeProperty);
        set => SetValue(HandlesEscapeProperty, value);
    }

    // En Loaded la navegación ya terminó y el BackStack dice de dónde viniste
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var text = UiText.Get(Navigator.CameFromRoom ? "Text.Header.BackToRoom" : "Text.Header.Back");
        BackLabel.Text = text;
        AutomationProperties.SetName(BackButton, text);
        if (!HandlesEscape) BackButton.KeyboardAccelerators.Clear();
    }

    private void OnBackClick(object sender, RoutedEventArgs e) => Navigator.GoBack();
}
