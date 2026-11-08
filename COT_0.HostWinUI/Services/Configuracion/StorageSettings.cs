// Sección "Storage" de appsettings.json
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>Carpeta dentro de %LocalAppData% y nombre de cada archivo JSON.</summary>
public record StorageSettings(string Folder, string UsersFile, string VisitsFile, string SessionsFile, string BackgroundsFile);
