// Pantalla de música: biblioteca con portadas, canción actual, controles y errores en InfoBar
using System.Collections.ObjectModel;
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Musica;
using COT_0.HostWinUI.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>Reemplaza al SongPicker del WPF (que era un selector flotante sobre el login).</summary>
public partial class MusicaViewModel : MusicControlsViewModel
{
    private readonly SongCatalog _catalog;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isErrorOpen;

    [ObservableProperty]
    private bool _isLoading;

    public MusicaViewModel(MusicService music, SongCatalog catalog) : base(music)
    {
        _catalog = catalog;
        music.Failed += ShowError;
    }

    public ObservableCollection<SongRowViewModel> Songs { get; } = [];

    /// <summary>Portada grande de la canción que suena (la misma de su fila).</summary>
    public BitmapImage? CurrentCover => Songs.FirstOrDefault(s => s.IsPlaying)?.Cover;

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
        OnPropertyChanged(nameof(CurrentCover));
    }

    // Carpeta que no existe, carpeta vacía o archivo que no se pudo abrir
    private void ShowError(MusicError error, string? path)
    {
        var detail = error == MusicError.Unreadable ? Path.GetFileName(path) : path ?? string.Empty;
        ErrorMessage = string.Format(UiText.Get($"Text.Music.Error.{error}"), detail);
        IsErrorOpen = true;
    }
}
