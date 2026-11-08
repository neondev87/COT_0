// Punto de entrada: arma los servicios, muestra la ventana y levanta la web en segundo plano
using COT_0.Core.Services;
using COT_0.HostWinUI.Services.Dialogos;
using COT_0.HostWinUI.Services.Musica;
using COT_0.HostWinUI.Services.Sala;
using COT_0.HostWinUI.Services.Tiempo;
using COT_0.HostWinUI.Services.Tunel;
using COT_0.HostWinUI.Services.Ventana;
using COT_0.HostWinUI.Views.ModelosDeVista;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace COT_0.HostWinUI;

public partial class App : Application
{
    private Window? _window;

    public App() => InitializeComponent();

    /// <summary>Contenedor de servicios de la app (lo arma OnLaunched).</summary>
    public static ServiceProvider Services { get; private set; } = null!;

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        UiThread.Attach(DispatcherQueue.GetForCurrentThread());
        Services = BuildServices();

        _window = new MainWindow();
        // Cerrar la ventana con sala activa pregunta primero (como SALIR)
        Services.GetRequiredService<CloseGuard>().Attach(_window);
        _window.Closed += (_, _) => Services.Dispose();
        _window.Activate();

        // Igual que el WPF: la música suena desde el login (si falta la carpeta, MusicaPage lo avisa)
        _ = Services.GetRequiredService<MusicService>().StartAsync();

        // Igual que el WPF: la web arranca mientras el host escribe su contraseña
        await TryStartWebServerAsync();
    }

    private static ServiceProvider BuildServices()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        return new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<ITunnelService, CloudflareTunnelService>()
            .AddSingleton<WebServerHost>()
            // Mismas instancias que usa el hub: salen del contenedor de la app web
            .AddSingleton(sp => sp.GetRequiredService<WebServerHost>().Services.GetRequiredService<IChatService>())
            .AddSingleton(sp => sp.GetRequiredService<WebServerHost>().Services.GetRequiredService<IRoomService>())
            .AddSingleton(sp => sp.GetRequiredService<WebServerHost>().Services.GetRequiredService<IRoomEmotes>())
            .AddSingleton<RoomSession>()
            .AddSingleton<ClockService>()
            .AddSingleton<ConfirmDialogService>()
            .AddSingleton<QrService>()
            .AddSingleton<SalaViewModel>()
            .AddSingleton<CloseGuard>()
            .AddSingleton<ConnectedUsersViewModel>()
            .AddSingleton<DashboardViewModel>()
            .AddSingleton<EmotesViewModel>()
            .AddSingleton<MusicLibrary>()
            .AddSingleton<CoverLibrary>()
            .AddSingleton<SongCatalog>()
            .AddSingleton<MusicService>()
            .AddSingleton<MusicaViewModel>()
            .AddSingleton<FirmaViewModel>()
            .BuildServiceProvider();
    }

    // Si falla aquí, abrir la sala lo reintenta (RoomSession.OpenAsync)
    private static async Task TryStartWebServerAsync()
    {
        try
        {
            await Services.GetRequiredService<WebServerHost>().StartAsync();
        }
        catch (Exception ex) when (ex is IOException or InvalidOperationException or System.Net.Sockets.SocketException)
        {
        }
    }
}
