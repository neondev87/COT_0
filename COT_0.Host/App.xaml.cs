using System.Windows;
using COT_0.Host.Services;

namespace COT_0.Host;

public partial class App : Application
{
    public static WebServerHost WebServer { get; } = new();

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Servidor web en segundo plano en http://localhost:5050
        await WebServer.StartAsync();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await WebServer.StopAsync();
        base.OnExit(e);
    }
}
