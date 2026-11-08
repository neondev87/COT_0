// Único reproductor de los fondos de video: en bucle, en silencio y sin controles de Windows
using COT_0.HostWinUI.Services.Tiempo;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace COT_0.HostWinUI.Services.Fondos;

/// <summary>
/// Reemplaza al LoopingVideo del WPF. Siempre es el mismo MediaPlayer (lo pinta el MediaPlayerElement
/// de CapaFondo); cambiar de fondo solo cambia su Source. Failed llega en el hilo de la UI.
/// </summary>
public sealed class FondoPlayer : IDisposable
{
    public FondoPlayer()
    {
        Player.CommandManager.IsEnabled = false;
        Player.MediaFailed += (_, _) => UiThread.Run(() => Failed?.Invoke());
    }

    public event Action? Failed;

    public MediaPlayer Player { get; } = new() { IsLoopingEnabled = true, IsMuted = true, AutoPlay = false };

    /// <summary>Ruta del video cargado; null si no hay.</summary>
    public string? Current { get; private set; }

    public void Play(string path)
    {
        if (!string.Equals(Current, path, StringComparison.OrdinalIgnoreCase))
        {
            Current = path;
            Player.Source = MediaSource.CreateFromUri(new Uri(Path.GetFullPath(path)));
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
}
