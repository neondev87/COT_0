// Sonidos del manual: aviso de cada notificación y estática en bucle en la despedida. SILENCIAR apaga los dos
using COT_0.HostWinUI.Services.Configuracion;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace COT_0.HostWinUI.Services.Manual;

/// <summary>
/// Reescrito del ManualSound del WPF con Windows.Media.Playback.MediaPlayer. Sin los "golpes" de estática
/// de cada pérdida de señal (dependían de SignalLossLayer, que no se migró).
/// </summary>
public sealed class ManualSound : IDisposable
{
    private readonly MediaPlayer _notice = new() { AutoPlay = false };
    private readonly MediaPlayer _static = new() { AutoPlay = false, IsLoopingEnabled = true };
    private readonly ManualAssets _assets;

    public ManualSound(ManualAssets assets)
    {
        _assets = assets;
        _notice.CommandManager.IsEnabled = false;
        _static.CommandManager.IsEnabled = false;
        _notice.Volume = AppConfig.Settings.Manual.NotificationVolume;
        _static.Volume = AppConfig.Settings.Manual.StaticVolume;
    }

    public bool IsMuted
    {
        get => _notice.IsMuted;
        set => _notice.IsMuted = _static.IsMuted = value;
    }

    /// <summary>Al abrir el manual, después de ManualAssets.RefreshAsync (los archivos que falten quedan mudos).</summary>
    public void Open()
    {
        _notice.Source = Source(_assets.NotificationSound);
        _static.Source = Source(_assets.StaticSound);
    }

    public void PlayNotice()
    {
        if (_notice.Source is null || IsMuted) return;
        _notice.PlaybackSession.Position = TimeSpan.Zero;
        _notice.Play();
    }

    public void StartStatic()
    {
        if (_static.Source is null) return;
        _static.PlaybackSession.Position = TimeSpan.Zero;
        _static.Play();
    }

    /// <summary>Al salir: corta todo, suelta los archivos y quita el silencio.</summary>
    public void Stop()
    {
        _notice.Source = null;
        _static.Source = null;
        IsMuted = false;
    }

    public void Dispose()
    {
        _notice.Dispose();
        _static.Dispose();
    }

    private static MediaSource? Source(string? path) => path is null ? null : MediaSource.CreateFromUri(new Uri(path));
}
