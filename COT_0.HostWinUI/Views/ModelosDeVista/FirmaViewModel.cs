// Firma Neondev del login: enlace al repo y controles chicos de la música (silencio, volumen y qué suena)
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Musica;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>Usa el mismo MusicService que MusicaPage, así que lo que cambies en una se ve en la otra.</summary>
public partial class FirmaViewModel(MusicService music) : MusicControlsViewModel(music)
{
    public bool IsMusicMissing => !IsMusicAvailable;

    [RelayCommand]
    private async Task OpenGitHubAsync()
    {
        if (Uri.TryCreate(AppConfig.Settings.Links.GitHub, UriKind.Absolute, out var url))
            await Windows.System.Launcher.LaunchUriAsync(url);
    }

    protected override void OnMusicStateChanged()
    {
        base.OnMusicStateChanged();
        OnPropertyChanged(nameof(IsMusicMissing));
    }
}
