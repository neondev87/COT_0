using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Guarda el volumen entre sesiones en Audio:SettingsFile
public class VolumeStore(IConfiguration configuration)
{
    private readonly string _path = Environment.ExpandEnvironmentVariables(configuration["Audio:SettingsFile"] ?? string.Empty);

    public double Load(double fallback)
    {
        if (string.IsNullOrWhiteSpace(_path) || !File.Exists(_path)) return fallback;
        try
        {
            return JsonSerializer.Deserialize<AudioSettings>(File.ReadAllText(_path))?.Volume ?? fallback;
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            return fallback;
        }
    }

    // Sincrónico: se llama al cerrar la app, cuando ya no corre el dispatcher
    public void Save(double volume)
    {
        if (string.IsNullOrWhiteSpace(_path)) return;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, JsonSerializer.Serialize(new AudioSettings(volume)));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
        }
    }

    private sealed record AudioSettings(double Volume);
}
