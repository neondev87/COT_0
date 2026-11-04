using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.ViewModels;

// Selector de canciones estilo rofi: buscador, flechas para moverse y Enter o clic para reproducir
public partial class SongPickerViewModel : ObservableObject
{
    private readonly MusicService _music;
    private readonly SongCatalog _catalog;
    private IReadOnlyList<SongItem> _all = [];

    [ObservableProperty]
    private bool _isOpen;

    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private IReadOnlyList<SongItem> _songs = [];

    [ObservableProperty]
    private SongItem? _selected;

    public SongPickerViewModel(MusicService music, SongCatalog catalog)
    {
        _music = music;
        _catalog = catalog;
        music.TrackChanged += MarkPlaying;
    }

    [RelayCommand]
    private async Task OpenAsync()
    {
        Query = string.Empty;
        IsOpen = true;
        _all = await _catalog.LoadAsync();
        MarkPlaying();
        Filter();
    }

    [RelayCommand]
    private void Close() => IsOpen = false;

    [RelayCommand]
    private void Previous() => Move(-1);

    [RelayCommand]
    private void Next() => Move(1);

    // Sin parámetro (Enter) reproduce la seleccionada
    [RelayCommand]
    private void Play(SongItem? song)
    {
        song ??= Selected;
        if (song is null) return;
        _music.Play(song.Path);
        IsOpen = false;
    }

    partial void OnQueryChanged(string value) => Filter();

    private void Filter()
    {
        Songs = _all.Where(song => song.Title.Contains(Query, StringComparison.OrdinalIgnoreCase)).ToList();
        Selected = Songs.FirstOrDefault(song => song.IsPlaying) ?? Songs.FirstOrDefault();
    }

    private void Move(int step)
    {
        if (Songs.Count == 0) return;
        var index = Selected is null ? 0 : Songs.ToList().IndexOf(Selected) + step;
        Selected = Songs[Math.Clamp(index, 0, Songs.Count - 1)];
    }

    private void MarkPlaying()
    {
        foreach (var song in _all)
            song.IsPlaying = string.Equals(song.Path, _music.Current, StringComparison.OrdinalIgnoreCase);
    }
}
