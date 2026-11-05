using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Sonidos del manual: aviso de cada notificación, estática en bucle de la despedida y golpes de estática
// en cada pérdida de señal. SILENCIAR apaga todo
public class ManualSound
{
    private readonly MediaPlayer _notice = new();
    private readonly MediaPlayer _static = new();
    private readonly MediaPlayer _hit = new();
    private readonly DispatcherTimer _hitEnd = new();
    private readonly bool _hasNotice;
    private readonly bool _hasStatic;
    private bool _isStaticOn;

    public ManualSound(ManualAssets assets, IConfiguration configuration)
    {
        _notice.Volume = configuration.GetValue<double>("Manual:NotificationVolume");
        _static.Volume = configuration.GetValue<double>("Manual:StaticVolume");
        _hasNotice = Open(_notice, assets.NotificationSound);
        _hit.Volume = configuration.GetValue<double>("Manual:StaticHitVolume");
        _hasStatic = Open(_static, assets.StaticSound);
        Open(_hit, assets.StaticSound);
        _hitEnd.Tick += (_, _) => StopHit();
        _static.MediaEnded += (_, _) =>
        {
            _static.Position = TimeSpan.Zero;
            _static.Play();
        };
    }

    public bool IsMuted
    {
        get => _notice.IsMuted;
        set => _notice.IsMuted = _static.IsMuted = _hit.IsMuted = value;
    }

    public void PlayNotice()
    {
        if (!_hasNotice || IsMuted) return;
        _notice.Position = TimeSpan.Zero;
        _notice.Play();
    }

    public void StartStatic()
    {
        if (!_hasStatic || _isStaticOn) return;
        _isStaticOn = true;
        _static.Position = TimeSpan.Zero;
        _static.Play();
    }

    // Un tramo al azar del audio de estática, solo lo que dura la ráfaga
    public void PlayStaticHit(TimeSpan duration)
    {
        if (!_hasStatic || IsMuted) return;
        var length = _hit.NaturalDuration.HasTimeSpan ? _hit.NaturalDuration.TimeSpan - duration : TimeSpan.Zero;
        _hit.Position = length > TimeSpan.Zero ? length * Random.Shared.NextDouble() : TimeSpan.Zero;
        _hit.Play();
        _hitEnd.Stop();
        _hitEnd.Interval = duration;
        _hitEnd.Start();
    }

    // Al cerrar el manual: corta todo y deja el silencio como estaba al abrir
    public void Stop()
    {
        _isStaticOn = false;
        _static.Stop();
        _notice.Stop();
        StopHit();
        IsMuted = false;
    }

    private void StopHit()
    {
        _hitEnd.Stop();
        _hit.Stop();
    }

    private static bool Open(MediaPlayer player, Uri? file)
    {
        if (file is null) return false;
        player.Open(file);
        return true;
    }
}
