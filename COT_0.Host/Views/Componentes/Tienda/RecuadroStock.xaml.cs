using System.Windows.Input;
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>Stock de un usuario: la página le pasa el UserStockViewModel abierto y el comando para cerrar</summary>
public sealed partial class RecuadroStock : UserControl
{
    public static readonly DependencyProperty StockProperty = DependencyProperty.Register(
        nameof(Stock), typeof(UserStockViewModel), typeof(RecuadroStock), new PropertyMetadata(null));

    public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(
        nameof(CloseCommand), typeof(ICommand), typeof(RecuadroStock), new PropertyMetadata(null));

    public RecuadroStock() => InitializeComponent();

    public UserStockViewModel? Stock
    {
        get => (UserStockViewModel?)GetValue(StockProperty);
        set => SetValue(StockProperty, value);
    }

    public ICommand? CloseCommand
    {
        get => (ICommand?)GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }
}
