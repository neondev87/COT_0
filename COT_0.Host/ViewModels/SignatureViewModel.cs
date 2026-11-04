using System.Diagnostics;
using System.IO;
using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.ViewModels;

// Firma Neondev: link al repo, nota musical (silencio / "Subir música"), volumen y selector de canciones
public partial class SignatureViewModel(
    MusicService music,
    MusicLibrary library,
    DialogService dialogs,
    SongPickerViewModel songs,
    IConfiguration configuration) : ObservableObject
{
    [ObservableProperty]
    private bool _isUploadOpen;

    public SongPickerViewModel Songs => songs;

    public bool IsMusicAvailable => music.IsAvailable;

    public bool IsMuted => music.IsMuted;

    // Mover el volumen quita el silencio
    public double Volume
    {
        get => music.Volume;
        set
        {
            music.Volume = value;
            music.IsMuted = false;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsMuted));
        }
    }

    // Con música silencia o quita el silencio; sin música muestra "Subir música"
    [RelayCommand]
    private void ToggleNote()
    {
        if (IsMusicAvailable)
        {
            music.IsMuted = !music.IsMuted;
            OnPropertyChanged(nameof(IsMuted));
            return;
        }
        IsUploadOpen = !IsUploadOpen;
    }

    [RelayCommand]
    private async Task UploadAsync()
    {
        var files = dialogs.PickFiles(Text("Music:FilterName"), library.Extensions);
        if (files.Count == 0) return;
        try
        {
            await library.ImportAsync(files);
            music.Start();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            dialogs.ShowError(Text("Music:UploadError"), ex.Message);
        }
        IsUploadOpen = false;
        OnPropertyChanged(nameof(IsMusicAvailable));
    }

    [RelayCommand]
    private void OpenGitHub()
    {
        var url = configuration["Links:GitHub"];
        if (string.IsNullOrWhiteSpace(url)) return;
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    private string Text(string key) => configuration["Texts:" + key] ?? string.Empty;
}
