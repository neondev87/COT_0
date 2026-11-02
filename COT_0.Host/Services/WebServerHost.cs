using COT_0.Web;
using Microsoft.AspNetCore.Builder;

namespace COT_0.Host.Services;

// Levanta COT_0.Web (Kestrel) en segundo plano dentro del proceso WPF
public class WebServerHost
{
    public const string Url = WebApp.DefaultUrl;

    private WebApplication? _app;

    public bool IsRunning => _app is not null;

    public async Task StartAsync()
    {
        if (_app is not null) return;

        // ContentRoot = carpeta del exe (ahí están appsettings.json y wwwroot)
        var app = WebApp.Create(url: Url, contentRoot: AppContext.BaseDirectory);
        try
        {
            await app.StartAsync();
        }
        catch
        {
            // Si falla (ej. puerto ocupado) queda listo para reintentar
            await app.DisposeAsync();
            throw;
        }
        _app = app;
    }

    public async Task StopAsync()
    {
        if (_app is null) return;

        await _app.StopAsync();
        await _app.DisposeAsync();
        _app = null;
    }
}
