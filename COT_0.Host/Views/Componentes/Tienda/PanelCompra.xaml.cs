using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>Panel COMPRA REALIZADA: la página le pasa el PurchaseSummaryViewModel de la tienda; los botones solo llaman a sus comandos</summary>
public sealed partial class PanelCompra : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(PurchaseSummaryViewModel), typeof(PanelCompra), new PropertyMetadata(null));

    public PanelCompra() => InitializeComponent();

    public PurchaseSummaryViewModel ViewModel
    {
        get => (PurchaseSummaryViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}
