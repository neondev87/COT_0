using System.Globalization;
using COT_0.Core.Services;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Lecturas de appsettings.json que repiten varias clases del Host
public static class ConfigurationExtensions
{
    // Ruta de un archivo o carpeta, resuelta contra Paths:MediaRoot si es relativa
    public static string GetMediaPath(this IConfiguration configuration, string key) =>
        MediaPath.Resolve(configuration[MediaPath.RootKey], configuration[key]);

    // Texto de la sección Texts (ej. "Dashboard:QrError"); vacío si falta
    public static string GetText(this IConfiguration configuration, string key) =>
        configuration["Texts:" + key] ?? string.Empty;

    // Ui:Culture para fechas y horas; si falta, la del sistema
    public static CultureInfo GetUiCulture(this IConfiguration configuration) =>
        CultureInfo.GetCultureInfo(configuration["Ui:Culture"] ?? CultureInfo.CurrentCulture.Name);
}
