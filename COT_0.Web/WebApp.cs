using COT_0.Web.Services;

namespace COT_0.Web;

// Arma la app web. La usa Program.cs (modo standalone) y COT_0.Host (en segundo plano).
public static class WebApp
{
    public const string DefaultUrl = "http://localhost:5050";

    public static WebApplication Create(string[]? args = null, string url = DefaultUrl, string? contentRoot = null)
    {
        var options = new WebApplicationOptions
        {
            Args = args ?? [],
            // Nombre fijo para que encuentre sus componentes aunque lo hospede otro exe
            ApplicationName = typeof(WebApp).Assembly.GetName().Name,
            ContentRootPath = contentRoot,
        };

        var builder = WebApplication.CreateBuilder(options);
        builder.WebHost.UseUrls(url);

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services.AddSignalR();
        builder.Services.AddChat(builder.Configuration, url);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
        }

        // Sin HTTPS local: cloudflared se encarga del https público
        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapGet("/", () => Results.Redirect("/join"));
        app.MapChat();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        return app;
    }
}
