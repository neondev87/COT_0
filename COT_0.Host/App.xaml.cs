using System.Windows;
using COT_0.Core.Services;
using COT_0.Host.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace COT_0.Host;

public partial class App : Application
{
    private ServiceProvider? _services;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        _services = BuildServices();

        await TryStartWebServerAsync();

        var login = _services.GetRequiredService<LoginWindow>();
        if (login.ShowDialog() != true)
        {
            Shutdown();
            return;
        }

        var main = _services.GetRequiredService<MainWindow>();
        MainWindow = main;
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        main.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_services is not null)
        {
            var webServer = _services.GetRequiredService<WebServerHost>();
            // Primero libera el túnel (mata cloudflared si sigue vivo), es sincrónico
            _services.Dispose();
            await webServer.StopAsync();
        }
        base.OnExit(e);
    }

    private static ServiceProvider BuildServices()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        return new ServiceCollection()
            .AddSingleton(configuration)
            .AddSingleton<IAuthService, ConfigAuthService>()
            .AddSingleton<ITunnelService, CloudflareTunnelService>()
            .AddSingleton<WebServerHost>()
            .AddSingleton<QrService>()
            .AddTransient<LoginWindow>()
            .AddTransient<MainWindow>()
            .BuildServiceProvider();
    }

    // Servidor web en segundo plano; si falla acá, "Generar QR" lo reintenta y muestra el error
    private async Task TryStartWebServerAsync()
    {
        try
        {
            await _services!.GetRequiredService<WebServerHost>().StartAsync();
        }
        catch (Exception)
        {
        }
    }
}
