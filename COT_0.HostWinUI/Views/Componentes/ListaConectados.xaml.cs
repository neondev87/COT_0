// Lista de conectados: toma su ViewModel del contenedor de servicios; la lógica vive ahí
using COT_0.HostWinUI.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Componentes;

public sealed partial class ListaConectados : UserControl
{
    public ConnectedUsersViewModel ViewModel { get; } = App.Services.GetRequiredService<ConnectedUsersViewModel>();

    public ListaConectados() => InitializeComponent();

    // El botón ✕ de cada fila trae su invitado en el DataContext
    private void OnKickClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: UserRowViewModel user })
            ViewModel.KickCommand.Execute(user);
    }
}
