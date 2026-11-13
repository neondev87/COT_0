// Tarjeta de un ticket (PUNTO DE VENTA y VER FACTURA): toma el ticket del DataContext y solo avisa los clics
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>
/// Quien la usa decide qué hacer con DESCARGAR PDF y ABRIR PDF: el sender de cada evento es esta tarjeta
/// y su ticket está en Ticket.
/// </summary>
public sealed partial class TarjetaTicket : UserControl
{
    public TarjetaTicket()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => Bindings.Update();
    }

    public TicketViewModel? Ticket => DataContext as TicketViewModel;

    /// <summary>DESCARGAR PDF de este ticket.</summary>
    public event RoutedEventHandler? DownloadClick;

    /// <summary>ABRIR PDF de este ticket con el visor de Windows.</summary>
    public event RoutedEventHandler? ViewClick;

    private void OnDownloadClick(object sender, RoutedEventArgs e) => DownloadClick?.Invoke(this, e);

    private void OnViewClick(object sender, RoutedEventArgs e) => ViewClick?.Invoke(this, e);
}
