// Punto de entrada: crea la ventana principal y la muestra
using Microsoft.UI.Xaml;

namespace COT_0.HostWinUI;

public partial class App : Application
{
    private Window? _window;

    public App() => InitializeComponent();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
