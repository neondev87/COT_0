using COT_0.Core.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Manual;

/// <summary>Registro del manual del invitado (/manual) y de sus audios</summary>
public static class ManualRegistration
{
    public static IServiceCollection AddManual(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ManualOptions>(config.GetSection(ManualOptions.Section));
        services.PostConfigure<ManualOptions>(o => o.AssetsFolder = MediaPath.Resolve(config[MediaPath.RootKey], o.AssetsFolder));
        services.AddScoped<IManualDemo, ManualDemo>();
        services.AddScoped<IManualTour, ManualTour>();
        return services;
    }

    // Audios desde la carpeta local, fuera del repo; si no existe, el manual funciona sin sonido
    public static WebApplication UseManualAssets(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<ManualOptions>>().Value;
        if (string.IsNullOrWhiteSpace(options.AssetsFolder) || !Directory.Exists(options.AssetsFolder)) return app;

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(options.AssetsFolder),
            RequestPath = options.AssetsPath.TrimEnd('/'),
        });
        return app;
    }
}
