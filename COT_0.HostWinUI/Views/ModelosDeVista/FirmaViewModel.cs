// Firma Neondev del login: controles chicos de la música (silencio, volumen y qué suena); el logo va aparte (LogoNeondev)
using COT_0.HostWinUI.Services.Musica;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>Usa el mismo MusicService que MusicaPage, así que lo que cambies en una se ve en la otra.</summary>
public partial class FirmaViewModel(MusicService music) : MusicControlsViewModel(music)
{
    public bool IsMusicMissing => !IsMusicAvailable;

    protected override void OnMusicStateChanged()
    {
        base.OnMusicStateChanged();
        OnPropertyChanged(nameof(IsMusicMissing));
    }
}
