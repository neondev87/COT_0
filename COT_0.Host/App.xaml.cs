using System.Windows;
using COT_0.Core.Services;
using COT_0.Host.Services;
using COT_0.Host.ViewModels;
using COT_0.Web.Services.Emotes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

    // El cierre ordenado ya lo hizo ShutdownCoordinator; esto es la red de seguridad:
    // Dispose mata cloudflared si sigue vivo y Kestrel termina con el proceso
    protected override void OnExit(ExitEventArgs e)
    {
        _services?.Dispose();
        base.OnExit(e);
    }

    private static IConfiguration LoadConfiguration() => new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    private static ServiceProvider BuildServices(IConfiguration configuration) => new ServiceCollection()
        .AddSingleton(configuration)
        .AddLogging(logging => logging.AddConfiguration(configuration.GetSection("Logging")).AddDebug())
        .AddSingleton<IAuthService, ConfigAuthService>()
        .AddSingleton<ITunnelService, CloudflareTunnelService>()
        .AddSingleton<WebServerHost>()
        // Mismas instancias que usa el hub: salen del contenedor de la app web
        .AddSingleton(sp => sp.GetRequiredService<WebServerHost>().Services.GetRequiredService<IChatService>())
        .AddSingleton(sp => sp.GetRequiredService<WebServerHost>().Services.GetRequiredService<IRoomService>())
        .AddSingleton(sp => sp.GetRequiredService<WebServerHost>().Services.GetRequiredService<IRoomEmotes>())
        .AddSingleton<RoomSession>()
        .AddSingleton<QrService>()
        .AddSingleton<ClockService>()
        .AddSingleton<MusicLibrary>()
        .AddSingleton<MusicService>()
        .AddSingleton<CoverLibrary>()
        .AddSingleton<CoverDecoder>()
        .AddSingleton<SongCatalog>()
        .AddSingleton<SongPickerViewModel>()
        .AddSingleton<DialogService>()
        .AddSingleton<BackgroundCatalog>()
        .AddSingleton<IShutdownCoordinator, ShutdownCoordinator>()
        .AddSingleton<WindowNavigator>()
        .AddSingleton<ScreenNavigator>()
        .AddSingleton<BackgroundPickerViewModel>()
        .AddSingleton<DashboardViewModel>()
        .AddSingleton<EmotesViewModel>()
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
