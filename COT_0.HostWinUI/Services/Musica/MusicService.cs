// Música de fondo con Windows.Media.Playback.MediaPlayer: arranca con Audio:DefaultFile (o la primera) y sigue en orden, en loop
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Tiempo;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace COT_0.HostWinUI.Services.Musica;

/// <summary>
/// Una sola instancia para toda la app: la firma del login y MusicaPage ven lo mismo.
/// Los eventos del reproductor llegan de otro hilo; StateChanged y Failed siempre se avisan en el hilo de la UI.
/// El volumen inicia siempre en Audio:DefaultVolume (0, silencio) y no se guarda entre sesiones.
/// Con volumen 0 la canción queda cargada pero en pausa (no avanza a la siguiente); empieza al subir el volumen.
/// </summary>
public sealed class MusicService : IDisposable
{
    private const double MaxVolume = 100;

    private readonly MediaPlayer _player = new();
    private readonly MusicLibrary _library;
    private IReadOnlyList<string> _tracks = [];
    private int _failures;
    private bool _playWhenAudible;

    public MusicService(MusicLibrary library)
    {
        _library = library;
        _player.CommandManager.IsEnabled = false;
        Volume = AppConfig.Settings.Audio.DefaultVolume;
        _player.MediaOpened += (_, _) => _failures = 0;
        _player.MediaEnded += (_, _) => UiThread.Run(() => _ = MoveAsync(1));
        _player.MediaFailed += (_, _) => UiThread.Run(() => _ = OnMediaFailedAsync());
        _player.PlaybackSession.PlaybackStateChanged += (_, _) => UiThread.Run(Notify);
    }

    /// <summary>Cambió la canción, la pausa, el silencio o el volumen.</summary>
    public event Action? StateChanged;

    /// <summary>Carpeta inexistente, sin canciones o archivo ilegible (con su ruta).</summary>
    public event Action<MusicError, string?>? Failed;

    public bool IsAvailable => _tracks.Count > 0;

    /// <summary>Ruta de la canción actual; null si no hay ninguna.</summary>
    public string? Current { get; private set; }

    public bool IsPlaying => _player.PlaybackSession.PlaybackState is MediaPlaybackState.Playing or MediaPlaybackState.Opening;

    /// <summary>0 a 100.</summary>
    public double Volume
    {
        get => _player.Volume * MaxVolume;
        set
        {
            _player.Volume = Math.Clamp(value, 0, MaxVolume) / MaxVolume;
            if (_playWhenAudible && _player.Volume > 0) Play();
            Notify();
        }
    }

    public bool IsMuted
    {
        get => _player.IsMuted;
        set
        {
            _player.IsMuted = value;
            Notify();
        }
    }

    /// <summary>Relee la carpeta y, si no estaba sonando nada, empieza.</summary>
    public async Task StartAsync()
    {
        if (!await ReloadAsync() || Current is not null) return;

        var defaultFile = AppConfig.Settings.Audio.DefaultFile;
        Open(_tracks.FirstOrDefault(t => Path.GetFileName(t).Equals(defaultFile, StringComparison.OrdinalIgnoreCase)) ?? _tracks[0]);
    }

    public async Task PlayAsync(string track)
    {
        if (await ReloadAsync()) Open(track);
    }

    public void TogglePlay()
    {
        if (Current is null) return;
        if (IsPlaying) _player.Pause();
        else Play();
    }

    public Task NextAsync() => MoveAsync(1);

    public Task PreviousAsync() => MoveAsync(-1);

    public void Dispose() => _player.Dispose();

    // Lee la carpeta de nuevo; false (y aviso) si no hay de dónde tocar
    private async Task<bool> ReloadAsync()
    {
        _tracks = await _library.GetTracksAsync();
        if (IsAvailable) return true;

        Failed?.Invoke(_library.FolderExists ? MusicError.NoTracks : MusicError.FolderMissing, _library.Folder);
        return false;
    }

    private async Task MoveAsync(int step)
    {
        if (!await ReloadAsync() || Current is null)
        {
            SetCurrent(null);
            return;
        }
        Open(step > 0 ? MusicLibrary.NextAfter(_tracks, Current) : MusicLibrary.PreviousBefore(_tracks, Current));
    }

    // Si un archivo no se puede abrir pasa al siguiente, sin girar en falso si fallan todos
    private async Task OnMediaFailedAsync()
    {
        Failed?.Invoke(MusicError.Unreadable, Current);
        if (++_failures < _tracks.Count) await MoveAsync(1);
        else SetCurrent(null);
    }

    private void Open(string track)
    {
        SetCurrent(track);
        _player.Source = MediaSource.CreateFromUri(new Uri(Path.GetFullPath(track)));
        if (_player.Volume > 0) Play();
        else _playWhenAudible = true;
    }

    private void Play()
    {
        _playWhenAudible = false;
        _player.Play();
    }

    private void SetCurrent(string? track)
    {
        Current = track;
        if (track is null)
        {
            _player.Source = null;
            _playWhenAudible = false;
        }
        Notify();
    }

    private void Notify() => StateChanged?.Invoke();
}
