// Capa del fondo: conecta el único reproductor de fondos con su MediaPlayerElement; la lógica vive en CapaFondoViewModel
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

public sealed partial class CapaFondo : UserControl
{
    public CapaFondoViewModel ViewModel { get; } = App.Services.GetRequiredService<CapaFondoViewModel>();

    public CapaFondo()
    {
        InitializeComponent();
        Video.SetMediaPlayer(ViewModel.Player);
    }
}
