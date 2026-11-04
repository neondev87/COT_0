using System.IO;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Música de fondo: arranca con Audio:DefaultFile (o la primera) y sigue en orden, en loop.
// El volumen inicia siempre en Audio:DefaultVolume; no se guarda entre sesiones
public class MusicService : IDisposable
{
    private const double MaxVolume = 100;

    private readonly MediaPlayer _player = new();
    private readonly MusicLibrary _library;
    private readonly string? _defaultFile;
    private IReadOnlyList<string> _tracks = [];
    private string? _current;
    private int _failures;

    public MusicService(MusicLibrary library, IConfiguration configuration)
    {
        _library = library;
        _defaultFile = configuration["Audio:DefaultFile"];
        Volume = configuration.GetValue<double>("Audio:DefaultVolume");
        _player.MediaOpened += (_, _) => _failures = 0;
        _player.MediaEnded += (_, _) => PlayNext();
        // Si un archivo no se puede abrir pasa al siguiente, sin girar en falso si fallan todos
        _player.MediaFailed += (_, _) =>
        {
            if (++_failures < _tracks.Count) PlayNext();
            else _current = null;
        };
    }

    public bool IsAvailable => _tracks.Count > 0;

    // 0 a 100
    public double Volume
    {
        get => _player.Volume * MaxVolume;
        set => _player.Volume = Math.Clamp(value, 0, MaxVolume) / MaxVolume;
    }

    // Relee la carpeta y, si no estaba sonando nada, empieza
    public void Start()
    {
        _library.EnsureFolder();
        _tracks = _library.GetTracks();
        if (_current is not null || _tracks.Count == 0) return;
        Play(_tracks.FirstOrDefault(IsDefault) ?? _tracks[0]);
    }

    public void Dispose()
    {
        _player.Close();
        GC.SuppressFinalize(this);
    }

    private void PlayNext()
    {
        _tracks = _library.GetTracks();
        if (_tracks.Count == 0 || _current is null)
        {
            _current = null;
            return;
        }
        Play(MusicLibrary.NextAfter(_tracks, _current));
    }

    private void Play(string track)
    {
        _current = track;
        _player.Open(new Uri(Path.GetFullPath(track)));
        _player.Play();
    }

    private bool IsDefault(string track) =>
        string.Equals(Path.GetFileName(track), _defaultFile, StringComparison.OrdinalIgnoreCase);
}
