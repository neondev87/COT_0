using COT_0.Host.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace COT_0.Host.Views.Pantallas;

/// <summary>Manual de Lain: conecta el video con su reproductor, pasa los atajos al ViewModel y lo arranca/para con la página</summary>
public sealed partial class ManualPage : Page
{
    public ManualViewModel ViewModel { get; } = App.Services.GetRequiredService<ManualViewModel>();

    public ManualPage()
    {
        InitializeComponent();
        LainVideo.SetMediaPlayer(ViewModel.VideoPlayer);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        SweepStoryboard.Begin();
        // El foco inicial queda en la página y no en ← INICIO: así ENTER llega a su atajo y avanza
        Focus(FocusState.Programmatic);
        await ViewModel.EnterAsync();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        SweepStoryboard.Stop();
        ViewModel.Leave();
    }

    private void OnStartKey(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) => Run(ViewModel.StartCommand, args);

    private void OnNextKey(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) => Run(ViewModel.NextCommand, args);

    private void OnPreviousKey(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) => Run(ViewModel.PreviousCommand, args);

    private void OnExitKey(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) => Run(ViewModel.ExitCommand, args);

    private static void Run(System.Windows.Input.ICommand command, KeyboardAcceleratorInvokedEventArgs args)
    {
        args.Handled = true;
        if (command.CanExecute(null)) command.Execute(null);
    }
}
