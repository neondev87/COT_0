using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace COT_0.Host.Views.Pantallas;

/// <summary>Pantalla de login: solo arranca las animaciones y le pasa el trabajo a LoginViewModel</summary>
public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel { get; } = new();

    public LoginPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        // Un solo manejo de ENTER para toda la pantalla; también si el campo ya marcó la tecla como atendida
        AddHandler(KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
    }

    // Solo desde un campo de texto: en botones y listas ENTER conserva su comportamiento normal
    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != VirtualKey.Enter || e.OriginalSource is not (TextBox or PasswordBox)) return;
        e.Handled = true;
        ViewModel.EnterCommand.Execute(null);
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        BlinkStoryboard.Begin();
        AmbientStoryboard.Begin();
        UserBox.Focus(FocusState.Programmatic);
        await ViewModel.LoadAsync();
    }
}
