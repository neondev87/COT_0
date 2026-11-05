using COT_0.Core.Services;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

public static class ConfigurationPaths
{
    // Ruta de un archivo o carpeta de la configuración, resuelta contra Paths:MediaRoot si es relativa
    public static string GetMediaPath(this IConfiguration configuration, string key) =>
        MediaPath.Resolve(configuration[MediaPath.RootKey], configuration[key]);
}
