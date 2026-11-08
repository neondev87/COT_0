// Registro de fallas de medios: una línea JSON por evento en %LocalAppData%\COT_0\registro-medios.jsonl
using System.Diagnostics;
using System.Text.Json;
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Registro;

/// <summary>
/// Guarda el ErrorCode/ExtendedErrorCode de MediaFailed y de las miniaturas para diagnosticar sin adivinar.
/// También lo manda a la salida de depuración. Si no se puede escribir el archivo, no rompe nada.
/// </summary>
public static class MediaLog
{
    private static readonly SemaphoreSlim _gate = new(1, 1);

    public static string FilePath => Path.Combine(JsonStore.Folder, AppConfig.Settings.Storage.MediaLogFile);

    public static string Hex(int hresult) => $"0x{hresult:X8}";

    public static Task WriteAsync(string source, string file, Exception ex) =>
        WriteAsync(source, file, Hex(ex.HResult), $"{ex.GetType().Name}: {ex.Message}");

    public static async Task WriteAsync(string source, string file, string? errorCode, string? detail)
    {
        var entry = new MediaLogEntry(DateTime.Now, source, Path.GetFileName(file), errorCode, detail);
        var line = JsonSerializer.Serialize(entry);
        Debug.WriteLine($"[COT_0 medios] {line}");

        await _gate.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(FilePath, line + Environment.NewLine);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Debug.WriteLine($"[COT_0 medios] no se pudo escribir el registro: {ex.Message}");
        }
        finally
        {
            _gate.Release();
        }
    }
}
