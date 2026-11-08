// Pantalla de login: solo arranca las animaciones y le pasa el trabajo a LoginViewModel
using COT_0.HostWinUI.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Pantallas;

public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel { get; } = new();

    public LoginPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        BlinkStoryboard.Begin();
        AmbientStoryboard.Begin();
        UserBox.Focus(FocusState.Programmatic);
        await ViewModel.LoadAsync();
    }
}
