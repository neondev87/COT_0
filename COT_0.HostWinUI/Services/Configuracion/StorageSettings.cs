// Sección "Storage" de appsettings.json
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>
/// Carpeta dentro de %LocalAppData%, nombre de cada archivo JSON, el registro de medios (JSON por línea)
/// y la carpeta de caché (cuadros de video generados; fuera del repo).
/// </summary>
public record StorageSettings(
    string Folder,
    string UsersFile,
    string VisitsFile,
    string SessionsFile,
    string BackgroundsFile,
    string MediaLogFile,
    string CacheFolder);
