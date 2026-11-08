// Lee appsettings.json una sola vez y deja la configuración a la mano de los servicios
using System.Text.Json;

namespace COT_0.HostWinUI.Services.Configuracion;

public record WindowSettings(int Width, int Height);

public record StorageSettings(string Folder, string UsersFile, string VisitsFile);

public record SecuritySettings(int SaltBytes, int HashBytes, int HashIterations);

public record ValidationSettings(int MaxUserLength, int MaxPasswordLength);

public record AppSettings(
    WindowSettings Window,
    StorageSettings Storage,
    SecuritySettings Security,
    ValidationSettings Validation);

/// <summary>Configuración de la app, cargada de appsettings.json junto al .exe.</summary>
public static class AppConfig
{
    private const string FileName = "appsettings.json";

    private static readonly Lazy<AppSettings> _settings = new(Load);

    public static AppSettings Settings => _settings.Value;

    private static AppSettings Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, FileName);
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<AppSettings>(json)
            ?? throw new InvalidOperationException($"No se pudo leer {FileName}.");
    }
}
