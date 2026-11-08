// Logo Neondev (login, dashboard y sala): abre el repositorio del proyecto en el navegador
using COT_0.HostWinUI.Services.Configuracion;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>La URL sale de Branding:RepoUrl; si no es una URL válida, el clic no hace nada.</summary>
public partial class LogoNeondevViewModel
{
    [RelayCommand]
    private async Task OpenRepoAsync()
    {
        if (Uri.TryCreate(AppConfig.Settings.Branding.RepoUrl, UriKind.Absolute, out var url))
            await Launcher.LaunchUriAsync(url);
    }
}
