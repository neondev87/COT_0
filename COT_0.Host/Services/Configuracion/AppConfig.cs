// Lee appsettings.json una sola vez al arrancar (async) y deja la configuración a la mano de los servicios
using System.Text.Json;
using COT_0.Host.Services.Almacenamiento;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services.Configuracion;

/// <summary>Configuración de la app, cargada de appsettings.json junto al .exe con LoadAsync en App.OnLaunched.</summary>
public static class AppConfig
{
    private const string FileName = "appsettings.json";

    private static AppSettings? _settings;
    private static byte[] _json = [];

    public static AppSettings Settings =>
        _settings ?? throw new InvalidOperationException($"{FileName} no se ha cargado: llama a AppConfig.LoadAsync al arrancar.");

    /// <summary>Lee el archivo una vez; las siguientes llamadas no vuelven a tocar el disco.</summary>
    public static async Task LoadAsync(CancellationToken ct = default)
    {
        if (_settings is not null) return;

        var path = Path.Combine(AppContext.BaseDirectory, FileName);
        using var buffer = new MemoryStream();
        await using (var file = AsyncFiles.OpenRead(path))
            await file.CopyToAsync(buffer, ct);

        _json = buffer.ToArray();
        _settings = JsonSerializer.Deserialize<AppSettings>(_json)
            ?? throw new InvalidOperationException($"No se pudo leer {FileName}.");
    }

    /// <summary>IConfiguration armado con los mismos bytes ya leídos (sin otra lectura de disco).</summary>
    public static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder().AddJsonStream(new MemoryStream(_json)).Build();
}
