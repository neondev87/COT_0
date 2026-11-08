namespace COT_0.Web;

/// <summary>Arma la app web. La usa Program.cs (modo standalone) y COT_0.Host (en segundo plano).</summary>
public static class WebApp
{
    public const string UrlKey = "Web:Url";

    // URL: Web:Url de appsettings.json (Host) o, en standalone, la del perfil de launchSettings.json
    public static WebApplication Create(string[]? args = null, string? contentRoot = null)
    {
        var options = new WebApplicationOptions
        {
            Args = args ?? [],
            // Nombre fijo para que encuentre sus componentes aunque lo hospede otro exe
            ApplicationName = typeof(WebApp).Assembly.GetName().Name,
            ContentRootPath = contentRoot,
        };

        var builder = WebApplication.CreateBuilder(options);
        var url = builder.Configuration[UrlKey] ?? builder.Configuration[WebHostDefaults.ServerUrlsKey]
            ?? throw new InvalidOperationException($"Falta {UrlKey} en appsettings.json.");
        builder.WebHost.UseUrls(url);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddSignalR();
        builder.Services.AddChat(builder.Configuration, url);
        builder.Services.AddManual(builder.Configuration);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
        }

        // Sin HTTPS local: cloudflared se encarga del https público
        app.UseStaticFiles();
        app.UseManualAssets();
        app.UseAntiforgery();

        app.MapGet("/", () => Results.Redirect("/join"));
        app.MapChat();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        return app;
    }
}
