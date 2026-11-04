using System.Windows;
using COT_0.Core.Services;
using COT_0.Host.Services;
using COT_0.Host.ViewModels;
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

        var configuration = LoadConfiguration();
        TextResources.Load(configuration, Resources);
        _services = BuildServices(configuration);

        await TryStartWebServerAsync();

        _services.GetRequiredService<MusicService>().Start();
        _services.GetRequiredService<WindowNavigator>().ShowLogin();
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

    private static IConfiguration LoadConfiguration() => new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    private static ServiceProvider BuildServices(IConfiguration configuration) => new ServiceCollection()
        .AddSingleton(configuration)
        .AddSingleton<IAuthService, ConfigAuthService>()
        .AddSingleton<ITunnelService, CloudflareTunnelService>()
        .AddSingleton<WebServerHost>()
        // Mismas instancias que usa el hub: salen del contenedor de la app web
        .AddSingleton(sp => sp.GetRequiredService<WebServerHost>().Services.GetRequiredService<IChatService>())
        .AddSingleton(sp => sp.GetRequiredService<WebServerHost>().Services.GetRequiredService<IRoomService>())
        .AddSingleton<QrService>()
        .AddSingleton<ClockService>()
        .AddSingleton<MusicLibrary>()
        .AddSingleton<VolumeStore>()
        .AddSingleton<MusicService>()
        .AddSingleton<DialogService>()
        .AddSingleton<BackgroundCatalog>()
        .AddSingleton<WindowNavigator>()
        .AddSingleton<ScreenNavigator>()
        .AddSingleton<BackgroundPickerViewModel>()
        .AddSingleton<DashboardViewModel>()
        .AddSingleton<RoomViewModel>()
        .AddSingleton<MainViewModel>()
        // Compartido por login y dashboard: mismo estado de música
        .AddSingleton<SignatureViewModel>()
        .AddTransient<LoginViewModel>()
        .AddTransient<LoginWindow>()
        .AddTransient<MainWindow>()
        .BuildServiceProvider();

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
