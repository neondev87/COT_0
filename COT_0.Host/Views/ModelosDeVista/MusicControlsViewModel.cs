// Base de los controles de música (firma del login y MusicaPage): canción actual, pausa, silencio y volumen
using System.Globalization;
using COT_0.Host.Services.Musica;
using COT_0.Host.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

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

    /// <summary>
    /// 0 a 100 como se ve en la barra: en silencio marca 0 sin perder el volumen guardado en MusicService.
    /// Subir la barra estando en silencio quita el silencio; el 0 que la barra regresa al silenciar se ignora.
    /// </summary>
    public double Volume
    {
        get => Music.IsMuted ? 0 : Music.Volume;
        set
        {
            if (Music.IsMuted)
            {
                if (value < 0.5) return;
                Music.IsMuted = false;
            }
            else if (Math.Abs(Music.Volume - value) < 0.5) return;
            Music.Volume = value;
        }
    }

    public string VolumeText => Math.Round(Volume).ToString(CultureInfo.InvariantCulture);

    /// <summary>Tooltip de la bocina: "Silenciar" o "Activar sonido".</summary>
    public string MuteText => UiText.Get(Music.IsMuted ? "Text.Volume.Unmute" : "Text.Volume.Mute");

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
        OnPropertyChanged(nameof(MuteText));
    }
}
