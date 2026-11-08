using COT_0.Host.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

/// <summary>Firma Neondev (login, dashboard y sala): toma su ViewModel del contenedor de servicios (comparte MusicService con MusicaPage)</summary>
public sealed partial class Firma : UserControl
{
    public FirmaViewModel ViewModel { get; } = App.Services.GetRequiredService<FirmaViewModel>();

    public Firma() => InitializeComponent();
}
