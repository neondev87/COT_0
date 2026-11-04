using System.Diagnostics;
using System.IO;
using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.ViewModels;

// Firma Neondev: link al repo, volumen de la música y "Subir música" cuando no hay canciones
public partial class SignatureViewModel(
    MusicService music,
    MusicLibrary library,
    DialogService dialogs,
    IConfiguration configuration) : ObservableObject
{
    [ObservableProperty]
    private bool _isUploadOpen;

    public bool IsMusicAvailable => music.IsAvailable;

    public double Volume
    {
        get => music.Volume;
        set
        {
            music.Volume = value;
            OnPropertyChanged();
        }
    }

    [RelayCommand]
    private void ToggleUpload()
    {
        if (!IsMusicAvailable) IsUploadOpen = !IsUploadOpen;
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
