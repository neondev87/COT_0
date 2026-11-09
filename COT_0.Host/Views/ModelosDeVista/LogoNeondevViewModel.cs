// Logo Neondev (login, dashboard y sala): abre el repositorio y, con un usuario dentro, pone su foto (o su inicial) en el avatar
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Perfiles;
using COT_0.Host.Services.Tiempo;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Views.Convertidores;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.System;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Sin sesión o en modo ADMIN queda el avatar de Neondev. Con un usuario del modo HOST: su foto de perfil,
/// o su inicial con color si no tiene. Se repinta al entrar, salir, cambiar de nombre o de foto.
/// La URL del repo sale de Branding:RepoUrl; si no es válida, el clic no hace nada.
/// </summary>
public partial class LogoNeondevViewModel : ObservableObject
{
    private int _version;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPhoto))]
    private BitmapImage? _photo;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasInitial))]
    private string? _userName;

    public LogoNeondevViewModel()
    {
        ActiveUser.Changed += () => UiThread.Run(() => _ = RefreshAsync());
        ProfilePhotos.Changed += name =>
        {
            if (ActiveUser.Is(name)) UiThread.Run(() => _ = RefreshAsync());
        };
    }

    public bool HasPhoto => Photo is not null;

    public bool HasInitial => UserName is not null && Photo is null;

    public string Initial => string.IsNullOrEmpty(UserName) ? string.Empty : char.ToUpperInvariant(UserName[0]).ToString();

    public int Tone => UserName is null ? 0 : UserTones.For(UserName);

    [RelayCommand]
    private async Task OpenRepoAsync()
    {
        if (Uri.TryCreate(AppConfig.Settings.Branding.RepoUrl, UriKind.Absolute, out var url))
            await Launcher.LaunchUriAsync(url);
    }

    // Si llegan dos avisos seguidos, solo cuenta el último
    private async Task RefreshAsync()
    {
        var version = ++_version;
        var name = ActiveUser.Name;
        var photo = name is null ? null : await ProfileImages.LoadAsync(name);
        if (version != _version) return;

        Photo = photo;
        UserName = name;
        OnPropertyChanged(nameof(Initial));
        OnPropertyChanged(nameof(Tone));
        OnPropertyChanged(nameof(HasInitial));
    }
}
