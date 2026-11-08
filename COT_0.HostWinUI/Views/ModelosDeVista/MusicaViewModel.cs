// Pantalla de música: biblioteca con portadas, canción actual, controles y errores en InfoBar
using System.Collections.ObjectModel;
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Musica;
using COT_0.HostWinUI.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>Reemplaza al SongPicker del WPF (que era un selector flotante sobre el login).</summary>
public partial class MusicaViewModel : MusicControlsViewModel
{
    private readonly SongCatalog _catalog;
    private readonly CoverImages _covers;
    private string? _coverPath;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isErrorOpen;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCover), nameof(IsCoverMissing))]
    private CoverArt? _currentCover;

    public MusicaViewModel(MusicService music, SongCatalog catalog, CoverImages covers) : base(music)
    {
        _catalog = catalog;
        _covers = covers;
        music.Failed += ShowError;
    }

    public ObservableCollection<SongRowViewModel> Songs { get; } = [];

    public bool HasCover => CurrentCover is not null;

    public bool IsCoverMissing => CurrentCover is null;

    /// <summary>Relee la carpeta cada vez que abres la pantalla (pudiste agregar canciones).</summary>
    public async Task LoadAsync()
    {
        IsLoading = true;
        IsErrorOpen = false;
        try
        {
            var size = AppConfig.Settings.Audio.CoverDecodeSize;
            Songs.Clear();
            foreach (var song in await _catalog.LoadAsync())
                Songs.Add(new SongRowViewModel(song, size));

            // Sin carpeta o sin canciones, MusicService avisa por Failed con el motivo
            await Music.StartAsync();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ShowError(MusicError.FolderMissing, null);
        }
        finally
        {
            IsLoading = false;
        }
        OnMusicStateChanged();
    }

    [RelayCommand]
    private Task PlayAsync(SongRowViewModel song) => Music.PlayAsync(song.Path);

    protected override void OnMusicStateChanged()
    {
        base.OnMusicStateChanged();
        foreach (var song in Songs)
            song.IsPlaying = string.Equals(song.Path, Music.Current, StringComparison.OrdinalIgnoreCase);
        _ = LoadCoverAsync(Songs.FirstOrDefault(s => s.IsPlaying)?.CoverPath);
    }

    // Portada grande de la que suena, decodificada a su tamaño; si cambias rápido de canción gana la última
    private async Task LoadCoverAsync(string? path)
    {
        if (path == _coverPath) return;
        _coverPath = path;
        var cover = path is null ? null : await _covers.LoadAsync(path);
        if (path == _coverPath) CurrentCover = cover;
    }

    // Carpeta que no existe, carpeta vacía o archivo que no se pudo abrir
    private void ShowError(MusicError error, string? path)
    {
        var detail = error == MusicError.Unreadable ? Path.GetFileName(path) : path ?? string.Empty;
        ErrorMessage = string.Format(UiText.Get($"Text.Music.Error.{error}"), detail);
        IsErrorOpen = true;
    }
}
