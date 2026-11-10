using COT_0.Host.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace COT_0.Host.Views.Pantallas;

/// <summary>Pantalla Emotes (mini tienda): solo arma la vista y pasa ESC y el clic en el velo; la lógica vive en EmotesScreenViewModel</summary>
public sealed partial class EmotesPage : Page
{
    public EmotesScreenViewModel ViewModel { get; } = App.Services.GetRequiredService<EmotesScreenViewModel>();

    public EmotesPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += (_, _) => ViewModel.Unload();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e) => await ViewModel.LoadAsync();

    private void OnEscapeInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        args.Handled = true;
        ViewModel.EscapeCommand.Execute(null);
    }

    private void OnVeilTapped(object sender, TappedRoutedEventArgs e) => ViewModel.CloseOverlayCommand.Execute(null);
}
