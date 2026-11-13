// PUNTO DE VENTA (modo ADMIN): pasa los clics de cada ticket al SalesViewModel
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>Punto de venta: la página le pasa su SalesViewModel; DESCARGAR PDF y ABRIR PDF solo llaman a sus comandos</summary>
public sealed partial class SeccionVentas : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(SalesViewModel), typeof(SeccionVentas), new PropertyMetadata(null));

    public SeccionVentas() => InitializeComponent();

    public SalesViewModel ViewModel
    {
        get => (SalesViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    // Cada tarjeta trae su ticket
    private void OnDownloadClick(object sender, RoutedEventArgs e)
    {
        if (sender is TarjetaTicket { Ticket: { } ticket }) ViewModel.DownloadCommand.Execute(ticket);
    }

    private void OnViewClick(object sender, RoutedEventArgs e)
    {
        if (sender is TarjetaTicket { Ticket: { } ticket }) ViewModel.ViewCommand.Execute(ticket);
    }
}
