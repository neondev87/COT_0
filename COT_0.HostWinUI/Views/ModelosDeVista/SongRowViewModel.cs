// Fila de la biblioteca de música: portada, título, duración y si es la que suena
using COT_0.HostWinUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>Se crea en el hilo de la UI; la portada se decodifica chica (DecodePixelWidth) y en segundo plano.</summary>
public partial class SongRowViewModel(Song song, int coverSize) : ObservableObject
{
    [ObservableProperty]
    private bool _isPlaying;

    public string Path { get; } = song.Path;
    public string Title { get; } = song.Title;
    public string Duration { get; } = song.Duration;

    /// <summary>Archivo de la portada (la grande se decodifica aparte, a su tamaño).</summary>
    public string? CoverPath { get; } = song.CoverPath;

    /// <summary>null si no hay ninguna portada en la carpeta.</summary>
    public BitmapImage? Cover { get; } = song.CoverPath is null
        ? null
        : new BitmapImage(new Uri(song.CoverPath)) { DecodePixelWidth = coverSize };
}
