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
            else SetCurrent(null);
        };
    }

    // Cambió la canción que suena (o dejó de sonar)
    public event Action? TrackChanged;

    public bool IsAvailable => _tracks.Count > 0;

    // Ruta de la canción que suena; null si no hay ninguna
    public string? Current { get; private set; }

    // 0 a 100
    public double Volume
    {
        get => _player.Volume * MaxVolume;
        set => _player.Volume = Math.Clamp(value, 0, MaxVolume) / MaxVolume;
    }

    public bool IsMuted
    {
        get => _player.IsMuted;
        set => _player.IsMuted = value;
    }

    // Relee la carpeta y, si no estaba sonando nada, empieza
    public void Start()
    {
        _library.EnsureFolder();
        _tracks = _library.GetTracks();
        if (Current is not null || _tracks.Count == 0) return;
        Play(_tracks.FirstOrDefault(IsDefault) ?? _tracks[0]);
    }

    public void Play(string track)
    {
        _tracks = _library.GetTracks();
        SetCurrent(track);
        _player.Open(new Uri(Path.GetFullPath(track)));
        _player.Play();
    }

    // Al salir: corta el audio y suelta el archivo
    public void Stop()
    {
        _player.Stop();
        _player.Close();
        SetCurrent(null);
    }

    public void Dispose()
    {
        _player.Close();
        GC.SuppressFinalize(this);
    }

    private void PlayNext()
    {
        _tracks = _library.GetTracks();
        if (_tracks.Count == 0 || Current is null)
        {
            SetCurrent(null);
            return;
        }
        Play(MusicLibrary.NextAfter(_tracks, Current));
    }

    private void SetCurrent(string? track)
    {
        Current = track;
        TrackChanged?.Invoke();
    }

    private bool IsDefault(string track) =>
        string.Equals(Path.GetFileName(track), _defaultFile, StringComparison.OrdinalIgnoreCase);
}
