// Firma del login: toma su ViewModel del contenedor de servicios (comparte MusicService con MusicaPage)
using COT_0.HostWinUI.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Componentes;

public sealed partial class Firma : UserControl
{
    public FirmaViewModel ViewModel { get; } = App.Services.GetRequiredService<FirmaViewModel>();

    public Firma() => InitializeComponent();
}
