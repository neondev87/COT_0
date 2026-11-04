using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.ViewModels;

// Fila del selector de canciones; Cover es null si no hay portada
public partial class SongItem(string path, string title, string duration, ImageSource? cover) : ObservableObject
{
    [ObservableProperty]
    private bool _isPlaying;

    public string Path { get; } = path;
    public string Title { get; } = title;
    public string Duration { get; } = duration;
    public ImageSource? Cover { get; } = cover;
}
