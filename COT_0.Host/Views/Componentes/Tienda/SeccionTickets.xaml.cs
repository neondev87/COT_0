using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>Sección TICKETS: la página le pasa su StoreTicketsViewModel; DESCARGAR PDF solo llama a su comando</summary>
public sealed partial class SeccionTickets : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(StoreTicketsViewModel), typeof(SeccionTickets), new PropertyMetadata(null));

    public SeccionTickets() => InitializeComponent();

    public StoreTicketsViewModel ViewModel
    {
        get => (StoreTicketsViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    // Cada botón trae su ticket en el DataContext
    private void OnDownloadClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: TicketViewModel item }) ViewModel.DownloadCommand.Execute(item);
    }
}
