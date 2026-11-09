// Único reproductor de video del manual: Lain en la tele durante los pasos y su despedida al final
using COT_0.Host.Services.Tiempo;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace COT_0.Host.Services.Manual;

/// <summary>
/// Un solo MediaPlayer en bucle y en silencio; la despedida
/// solo cambia su Source. Failed (con la ruta) llega en el hilo de la UI.
/// Sin Dispose: ManualPage lo detiene en Unloaded, que llega aun después de cerrar la ventana.
/// </summary>
public sealed class ManualVideo
{
    private string? _current;

    public ManualVideo()
    {
        Player.CommandManager.IsEnabled = false;
        Player.MediaFailed += (_, _) => UiThread.Run(() => Failed?.Invoke(_current ?? string.Empty));
    }

    public event Action<string>? Failed;

    public MediaPlayer Player { get; } = new() { IsLoopingEnabled = true, IsMuted = true, AutoPlay = false };

    /// <summary>null = sin video (el archivo no está).</summary>
    public void Play(string? path)
    {
        _current = path;
        Player.Source = path is null ? null : MediaSource.CreateFromUri(new Uri(path));
        if (path is not null) Player.Play();
    }

    public void Stop() => Play(null);
}
