using System.IO;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Música de fondo en loop (Audio:MusicPath); si no hay archivo queda deshabilitada
public class MusicService
{
    private const double MaxVolume = 100;

    private readonly MediaPlayer _player = new();
    private readonly string? _path;

    public MusicService(IConfiguration configuration)
    {
        _path = configuration["Audio:MusicPath"];
        Volume = configuration.GetValue("Audio:DefaultVolume", 60);
        _player.MediaEnded += (_, _) =>
        {
            _player.Position = TimeSpan.Zero;
            _player.Play();
        };
    }

    public bool IsAvailable => !string.IsNullOrWhiteSpace(_path) && File.Exists(_path);

    // 0 a 100
    public double Volume
    {
        get => _player.Volume * MaxVolume;
        set => _player.Volume = Math.Clamp(value, 0, MaxVolume) / MaxVolume;
    }

    public void Start()
    {
        if (!IsAvailable) return;
        _player.Open(new Uri(Path.GetFullPath(_path!)));
        _player.Play();
    }
}
