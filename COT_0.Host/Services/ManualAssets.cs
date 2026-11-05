using System.IO;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Videos y sonidos del manual, fuera del repo (rutas en Manual:*); null si el archivo no está
public class ManualAssets(IConfiguration configuration)
{
    public Uri? LainVideo => Resolve("LainVideo");

    public Uri? FarewellVideo => Resolve("FarewellVideo");

    public Uri? NotificationSound => Resolve("NotificationSound");

    public Uri? StaticSound => Resolve("StaticSound");

    private Uri? Resolve(string key)
    {
        var file = configuration["Manual:" + key];
        return !string.IsNullOrWhiteSpace(file) && File.Exists(file) ? new Uri(file) : null;
    }
}
