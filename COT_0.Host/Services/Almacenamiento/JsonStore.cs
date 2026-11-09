// Guarda y lee cualquier clase como JSON en %LocalAppData%\COT_0
using System.Text.Json;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Almacenamiento;

/// <summary>Almacén JSON genérico: un archivo por tipo de dato.</summary>
public static class JsonStore
{
    private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    /// <summary>Carpeta donde viven los .json (solo la ruta; EnsureFolderAsync la crea).</summary>
    public static string Folder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppConfig.Settings.Storage.Folder);

    /// <summary>Crea la carpeta si no existe, fuera del hilo de la UI (.NET no trae versión async).</summary>
    public static async Task<string> EnsureFolderAsync()
    {
        var folder = Folder;
        await Task.Run(() => Directory.CreateDirectory(folder));
        return folder;
    }

    /// <summary>Lee el archivo; si no existe o está vacío, te regresa un objeto nuevo.</summary>
    public static async Task<T> LoadAsync<T>(string fileName) where T : new()
    {
        var path = Path.Combine(Folder, fileName);
        if (!File.Exists(path)) return new T();

        await using var stream = AsyncFiles.OpenRead(path);
        if (stream.Length == 0) return new T();
        return await JsonSerializer.DeserializeAsync<T>(stream, _options) ?? new T();
    }

    /// <summary>true si el error viene de leer o escribir el archivo (o de un JSON dañado).</summary>
    public static bool IsStorageError(Exception ex) =>
        ex is IOException or JsonException or UnauthorizedAccessException or FormatException;

    /// <summary>Escribe el objeto completo, con sangría para que se pueda leer a mano.</summary>
    public static async Task SaveAsync<T>(string fileName, T data)
    {
        var path = Path.Combine(await EnsureFolderAsync(), fileName);
        await using var stream = AsyncFiles.Create(path);
        await JsonSerializer.SerializeAsync(stream, data, _options);
    }
}
