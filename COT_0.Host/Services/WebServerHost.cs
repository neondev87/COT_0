using COT_0.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Levanta COT_0.Web (Kestrel) en segundo plano dentro del proceso WPF
public class WebServerHost(IConfiguration configuration)
{
    private Lazy<WebApplication> _app = CreateLazy();
    private Task? _starting;

    // Web:Url de appsettings.json (la misma que usa la app web)
    public string Url => configuration[WebApp.UrlKey] ?? string.Empty;

    public bool IsRunning => _starting is { IsCompletedSuccessfully: true };

    // Contenedor de la app web: IChatService/IRoomService son las mismas instancias que usa el hub.
    // Si la app se está armando en segundo plano, espera a que termine (Lazy es seguro entre hilos)
    public IServiceProvider Services => _app.Value.Services;

    // Arma e inicia la web fuera del hilo de la UI; si ya arrancó (o está arrancando) reusa esa tarea
    public Task StartAsync()
    {
        if (_starting is null || _starting.IsFaulted || _starting.IsCanceled)
            _starting = Task.Run(() => _app.Value.StartAsync());
        return _starting;
    }

    public async Task StopAsync()
    {
        if (!_app.IsValueCreated) return;

        var app = _app.Value;
        // Espera un arranque en curso sin relanzar su error: si falló, no hay nada que parar
        if (_starting is not null) await Task.WhenAny(_starting);
        if (IsRunning) await app.StopAsync();
        await app.DisposeAsync();
        _app = CreateLazy();
        _starting = null;
    }

    // ContentRoot = carpeta del exe (ahí están appsettings.json y wwwroot)
    private static Lazy<WebApplication> CreateLazy() => new(() =>
        StartupTrace.Measure("  WebApp.Create", () => WebApp.Create(contentRoot: AppContext.BaseDirectory)));
}
