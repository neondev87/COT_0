using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>Punto de venta: la página le pasa su SalesViewModel; DESCARGAR PDF solo llama a su comando</summary>
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

    // Cada botón trae su ticket en el DataContext
    private void OnDownloadClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: TicketViewModel item }) ViewModel.DownloadCommand.Execute(item);
    }
}
