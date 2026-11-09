// Sección "Storage" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>
/// Carpeta dentro de %LocalAppData%, nombre de cada archivo JSON (también compras.json de la tienda y admin.json de los administradores), la carpeta de fotos de perfil, el registro de medios (JSON por línea)
/// y la carpeta de caché (cuadros de video generados; fuera del repo).
/// </summary>
public record StorageSettings(
    string Folder,
    string UsersFile,
    string VisitsFile,
    string SessionsFile,
    string BackgroundsFile,
    string MediaLogFile,
    string PurchasesFile,
    string AdminsFile,
    string ProfilesFolder,
    string CacheFolder);
