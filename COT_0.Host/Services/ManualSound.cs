using System.Windows.Media;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Sonidos del manual: aviso de cada notificación y estática en bucle de la despedida. SILENCIAR apaga ambos
public class ManualSound
{
    private readonly MediaPlayer _notice = new();
    private readonly MediaPlayer _static = new();
    private readonly bool _hasNotice;
    private readonly bool _hasStatic;
    private bool _isStaticOn;

    public ManualSound(ManualAssets assets, IConfiguration configuration)
    {
        _notice.Volume = configuration.GetValue<double>("Manual:NotificationVolume");
        _static.Volume = configuration.GetValue<double>("Manual:StaticVolume");
        _hasNotice = Open(_notice, assets.NotificationSound);
        _hasStatic = Open(_static, assets.StaticSound);
        _static.MediaEnded += (_, _) =>
        {
            _static.Position = TimeSpan.Zero;
            _static.Play();
        };
    }

    public bool IsMuted
    {
        get => _notice.IsMuted;
        set => _notice.IsMuted = _static.IsMuted = value;
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

    // Al cerrar el manual: corta todo y deja el silencio como estaba al abrir
    public void Stop()
    {
        _isStaticOn = false;
        _static.Stop();
        _notice.Stop();
        IsMuted = false;
    }

    private static bool Open(MediaPlayer player, Uri? file)
    {
        if (file is null) return false;
        player.Open(file);
        return true;
    }
}
