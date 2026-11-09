// Punto de entrada: arma los servicios, muestra la ventana y levanta la web en segundo plano
using COT_0.Core.Services;
using COT_0.Host.Services.Administradores;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Dialogos;
using COT_0.Host.Services.Fondos;
using COT_0.Host.Services.Manual;
using COT_0.Host.Services.Musica;
using COT_0.Host.Services.Sala;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Tiempo;
using COT_0.Host.Services.Tunel;
using COT_0.Host.Services.Ventana;
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace COT_0.Host;

public partial class App : Application
{
    private Window? _window;

    public App() => InitializeComponent();

    /// <summary>Contenedor de servicios de la app (lo arma OnLaunched).</summary>
    public static ServiceProvider Services { get; private set; } = null!;

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        UiThread.Attach(DispatcherQueue.GetForCurrentThread());
        // Lee appsettings.json sin trabar la UI; todo lo demás depende de esto
        await AppConfig.LoadAsync();
        Services = BuildServices();

        _window = new MainWindow();
        // Cerrar la ventana con sala activa pregunta primero (como SALIR)
        // Apaga en orden con AppShutdown; el contenedor no se libera en Window.Closed (la vista aún usa los reproductores)
        Services.GetRequiredService<CloseGuard>().Attach(_window);
        _window.Activate();

        // La música suena desde el login (si falta la carpeta, MusicaPage lo avisa)
        _ = Services.GetRequiredService<MusicService>().StartAsync();

        // La web arranca mientras el host escribe su contraseña
        await TryStartWebServerAsync();
    }

    private static ServiceProvider BuildServices()
    {
        var configuration = AppConfig.BuildConfiguration();

        return new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton<ITunnelService, CloudflareTunnelService>()
            .AddSingleton<WebServerHost>()
            // Mismas instancias que usa el hub: salen del contenedor de la app web
            .AddSingleton(sp => sp.GetRequiredService<WebServerHost>().Services.GetRequiredService<IChatService>())
            .AddSingleton(sp => sp.GetRequiredService<WebServerHost>().Services.GetRequiredService<IRoomService>())
            .AddSingleton(sp => sp.GetRequiredService<WebServerHost>().Services.GetRequiredService<IRoomEmotes>())
            .AddSingleton<PurchaseStore>()
            .AddSingleton<PurchaseService>()
            .AddSingleton<EmoteShopCatalog>()
            .AddSingleton<ChatEmoteImport>()
            .AddSingleton<RoomSession>()
            .AddSingleton<ClockService>()
            .AddSingleton<ConfirmDialogService>()
            .AddSingleton<AdminPasswordDialogService>()
            .AddSingleton<AdminStore>()
            .AddSingleton<AdminService>()
            .AddTransient<AdminLoginViewModel>()
            .AddTransient<LoginModeViewModel>()
            .AddTransient<LoginViewModel>()
            .AddTransient<SalesViewModel>()
            .AddTransient<AdminsViewModel>()
            .AddSingleton<QrService>()
            .AddSingleton<SalaViewModel>()
            .AddSingleton<AppShutdown>()
            .AddSingleton<CloseGuard>()
            .AddSingleton<ConnectedUsersViewModel>()
            .AddSingleton<DashboardViewModel>()
            .AddSingleton<EmotesViewModel>()
            .AddTransient<StoreUsersViewModel>()
            .AddTransient<StoreShopViewModel>()
            .AddTransient<StoreTicketsViewModel>()
            .AddTransient<EmotesScreenViewModel>()
            .AddSingleton<MusicLibrary>()
            .AddSingleton<CoverLibrary>()
            .AddSingleton<SongCatalog>()
            .AddSingleton<CoverImages>()
            .AddSingleton<MusicService>()
            .AddSingleton<MusicaViewModel>()
            .AddSingleton<FirmaViewModel>()
            .AddSingleton<LogoNeondevViewModel>()
            .AddSingleton<IBackgroundCatalog, FondoCatalog>()
            .AddSingleton<FondoStore>()
            .AddSingleton<FondoService>()
            .AddSingleton<FfmpegFrames>()
            .AddSingleton<ShellThumbnailFrames>()
            .AddSingleton<CompositionFrames>()
            .AddSingleton<FondoFrameCache>()
            .AddSingleton<FondoImages>()
            .AddSingleton<FondoPlayer>()
            .AddSingleton<CapaFondoViewModel>()
            .AddSingleton<FondosViewModel>()
            .AddSingleton<ManualAssets>()
            .AddSingleton<ManualSound>()
            .AddSingleton<ManualVideo>()
            .AddTransient<ManualTimeline>()
            .AddTransient<ManualViewModel>()
            .AddTransient<ManualDemoViewModel>()
            .BuildServiceProvider();
    }

    // Si falla aquí no se pierde: abrir la sala revisa el puerto otra vez y muestra ese mismo error (RoomSession.OpenAsync)
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
