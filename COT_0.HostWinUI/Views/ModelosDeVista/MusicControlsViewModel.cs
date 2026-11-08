// Base de los controles de música (firma del login y MusicaPage): canción actual, pausa, silencio y volumen
using System.Globalization;
using COT_0.HostWinUI.Services.Musica;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>
/// Los dos ViewModels leen el mismo MusicService: así la firma y la pantalla de música siempre van sincronizadas.
/// StateChanged llega en el hilo de la UI.
/// </summary>
public abstract partial class MusicControlsViewModel : ObservableObject
{
    protected MusicControlsViewModel(MusicService music)
    {
        Music = music;
        music.StateChanged += OnMusicStateChanged;
    }

    public bool IsMusicAvailable => Music.IsAvailable;

    public bool IsPlaying => Music.IsPlaying;

    public bool IsMuted => Music.IsMuted;

    /// <summary>Nombre del archivo sin extensión; vacío si no suena nada.</summary>
    public string TrackTitle => Music.Current is { } track ? Path.GetFileNameWithoutExtension(track) : string.Empty;

    /// <summary>0 a 100. Mover el volumen quita el silencio.</summary>
    public double Volume
    {
        get => Music.Volume;
        set
        {
            if (Math.Abs(Music.Volume - value) < 0.5) return;
            Music.IsMuted = false;
            Music.Volume = value;
        }
    }

    public string VolumeText => Math.Round(Music.Volume).ToString(CultureInfo.InvariantCulture);

    protected MusicService Music { get; }

    [RelayCommand]
    private void ToggleMute() => Music.IsMuted = !Music.IsMuted;

    [RelayCommand]
    private void TogglePlay() => Music.TogglePlay();

    [RelayCommand]
    private Task NextAsync() => Music.NextAsync();

    [RelayCommand]
    private Task PreviousAsync() => Music.PreviousAsync();

    /// <summary>Lo que cada pantalla agrega al cambiar la música (marcar la fila que suena, etc.).</summary>
    protected virtual void OnMusicStateChanged()
    {
        OnPropertyChanged(nameof(IsMusicAvailable));
        OnPropertyChanged(nameof(IsPlaying));
        OnPropertyChanged(nameof(IsMuted));
        OnPropertyChanged(nameof(TrackTitle));
        OnPropertyChanged(nameof(Volume));
        OnPropertyChanged(nameof(VolumeText));
    }
}
