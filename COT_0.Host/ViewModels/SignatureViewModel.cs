using System.Diagnostics;
using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.ViewModels;

// Firma Neondev: link al repo y volumen de la música
public partial class SignatureViewModel(MusicService music, IConfiguration configuration) : ObservableObject
{
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
    private void OpenGitHub()
    {
        var url = configuration["Links:GitHub"];
        if (string.IsNullOrWhiteSpace(url)) return;
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
