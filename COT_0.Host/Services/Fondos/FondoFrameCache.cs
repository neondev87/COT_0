// Cuadro fijo de cada video en %LocalAppData%\COT_0\cache\fondos\<hash>.jpg: se genera una sola vez por video
using System.Collections.Concurrent;
using COT_0.Host.Services.Cache;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Registro;

namespace COT_0.Host.Services.Fondos;

/// <summary>
/// Lo usan la miniatura de FONDOS y el cuadro fijo del fondo. Intentos en orden: ffmpeg, miniatura de Windows y
/// MediaComposition. El nombre sale de ruta + tamaño + fecha (CacheFolders.KeyOf): si cambias el video, se regenera.
/// Si dos pantallas lo piden a la vez, esperan la misma generación.
/// </summary>
public class FondoFrameCache(FfmpegFrames ffmpeg, ShellThumbnailFrames shell, CompositionFrames composition)
{
    private const string Extension = ".jpg";
    private const string PartialExtension = ".part.jpg";

    private readonly ConcurrentDictionary<string, Task<string?>> _pending = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Ruta del JPEG, o null si ningún intento pudo sacar un cuadro (queda en el registro de medios).</summary>
    public async Task<string?> GetAsync(string video)
    {
        var task = _pending.GetOrAdd(video, CreateAsync);
        try
        {
            return await task;
        }
        finally
        {
            _pending.TryRemove(new KeyValuePair<string, Task<string?>>(video, task));
        }
    }

    private async Task<string?> CreateAsync(string video)
    {
        var target = await Task.Run(() => TargetOf(video));
        if (target is null) return null;
        if (await Task.Run(() => File.Exists(target))) return target;

        var partial = Path.ChangeExtension(target, PartialExtension);
        foreach (var attempt in new Func<string, string, Task<bool>>[] { ffmpeg.TryExtractAsync, shell.TryExtractAsync, composition.TryExtractAsync })
        {
            if (!await attempt(video, partial)) continue;
            await Task.Run(() => File.Move(partial, target, overwrite: true));
            return target;
        }
        await MediaLog.WriteAsync("cuadro-fijo", video, null, "ningún método pudo sacar un cuadro");
        return null;
    }

    private static string? TargetOf(string video)
    {
        var file = new FileInfo(video);
        if (!file.Exists) return null;
        var folder = CacheFolders.Get(AppConfig.Settings.Background.CacheFolder);
        return Path.Combine(folder, CacheFolders.KeyOf(file) + Extension);
    }
}
