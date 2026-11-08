// Inicio del host: toma su ViewModel del contenedor de servicios; la lógica vive en DashboardViewModel
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Pantallas;

public sealed partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel { get; } = App.Services.GetRequiredService<DashboardViewModel>();

    public DashboardPage() => InitializeComponent();
}
