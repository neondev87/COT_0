using COT_0.Web;
using Microsoft.AspNetCore.Builder;

namespace COT_0.Host.Services;

// Levanta COT_0.Web (Kestrel) en segundo plano dentro del proceso WPF
public class WebServerHost
{
    public const string Url = WebApp.DefaultUrl;

    private WebApplication? _app;
    private bool _started;

    public bool IsRunning => _started;

    // Contenedor de la app web: IChatService/IRoomService son las mismas instancias que usa el hub
    public IServiceProvider Services => App.Services;

    // ContentRoot = carpeta del exe (ahí están appsettings.json y wwwroot)
    private WebApplication App => _app ??= WebApp.Create(url: Url, contentRoot: AppContext.BaseDirectory);

    public async Task StartAsync()
    {
        if (_started) return;
        await App.StartAsync();
        _started = true;
    }

    public async Task StopAsync()
    {
        if (_app is null) return;

        if (_started) await _app.StopAsync();
        await _app.DisposeAsync();
        _app = null;
        _started = false;
    }
}
