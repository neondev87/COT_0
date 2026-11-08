// Único reproductor de los fondos de video: en bucle, en silencio y sin controles de Windows
using COT_0.Host.Models;
using COT_0.Host.Services.Registro;
using COT_0.Host.Services.Tiempo;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace COT_0.Host.Services.Fondos;

/// <summary>
/// Siempre es el mismo MediaPlayer (lo pinta el MediaPlayerElement
/// de CapaFondo); cambiar de fondo solo cambia su Source. El video se abre como StorageFile (no como Uri),
/// así los nombres con "：", "＂", corchetes o espacios no pasan por el escape de una URL.
/// Failed llega en el hilo de la UI con la causa y el código (también quedan en el registro de medios).
/// </summary>
public sealed class FondoPlayer : IDisposable
{
    private const string LogSource = "video-fondo";

    public FondoPlayer()
    {
        Player.CommandManager.IsEnabled = false;
        Player.MediaFailed += OnMediaFailed;
    }

    public event Action<FondoError, string>? Failed;

    public MediaPlayer Player { get; } = new() { IsLoopingEnabled = true, IsMuted = true, AutoPlay = false };

    /// <summary>Ruta del video cargado; null si no hay.</summary>
    public string? Current { get; private set; }

    public async Task PlayAsync(string path)
    {
        if (!string.Equals(Current, path, StringComparison.OrdinalIgnoreCase))
        {
            Current = path;
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(path);
                if (Current != path) return;
                Player.Source = MediaSource.CreateFromStorageFile(file);
            }
            catch (Exception ex) when (FondoFailures.IsMediaException(ex))
            {
                await MediaLog.WriteAsync(LogSource, path, ex);
                Failed?.Invoke(FondoFailures.FromException(ex), MediaLog.Hex(ex.HResult));
                return;
            }
        }
        Player.Play();
    }

    /// <summary>Fuera del dashboard, la sala y el selector el video se pausa (no se descarga).</summary>
    public void Pause()
    {
        if (Current is not null) Player.Pause();
    }

    /// <summary>Suelta el archivo (fondo de imagen, cuadro fijo o sin fondo).</summary>
    public void Stop()
    {
        if (Current is null) return;
        Current = null;
        Player.Source = null;
    }

    public void Dispose() => Player.Dispose();

    // Error y ExtendedErrorCode tal cual al registro; la vista recibe la causa ya traducida
    private void OnMediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs e)
    {
        var hresult = e.ExtendedErrorCode?.HResult ?? 0;
        var code = MediaLog.Hex(hresult);
        _ = MediaLog.WriteAsync(LogSource, Current ?? string.Empty, code, $"{e.Error}: {e.ErrorMessage}");
        var error = FondoFailures.FromMedia(e.Error, hresult);
        UiThread.Run(() => Failed?.Invoke(error, code));
    }
}
