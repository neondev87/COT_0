// VER FACTURA de un usuario: la página le pasa el UserInvoicesViewModel abierto y el comando para cerrar
using System.Windows.Input;
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>Los botones de cada ticket solo llaman a los comandos del ViewModel.</summary>
public sealed partial class RecuadroFacturas : UserControl
{
    public static readonly DependencyProperty InvoicesProperty = DependencyProperty.Register(
        nameof(Invoices), typeof(UserInvoicesViewModel), typeof(RecuadroFacturas), new PropertyMetadata(null));

    public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(
        nameof(CloseCommand), typeof(ICommand), typeof(RecuadroFacturas), new PropertyMetadata(null));

    public RecuadroFacturas() => InitializeComponent();

    public UserInvoicesViewModel? Invoices
    {
        get => (UserInvoicesViewModel?)GetValue(InvoicesProperty);
        set => SetValue(InvoicesProperty, value);
    }

    public ICommand? CloseCommand
    {
        get => (ICommand?)GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    private void OnDownloadClick(object sender, RoutedEventArgs e)
    {
        if (sender is TarjetaTicket { Ticket: { } ticket }) Invoices?.DownloadCommand.Execute(ticket);
    }

    private void OnViewClick(object sender, RoutedEventArgs e)
    {
        if (sender is TarjetaTicket { Ticket: { } ticket }) Invoices?.ViewCommand.Execute(ticket);
    }
}
