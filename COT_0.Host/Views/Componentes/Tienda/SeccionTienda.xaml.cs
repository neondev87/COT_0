using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>Sección TIENDA: la página le pasa su StoreShopViewModel; COMPRAR y ENTER solo llaman a sus comandos</summary>
public sealed partial class SeccionTienda : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(StoreShopViewModel), typeof(SeccionTienda), new PropertyMetadata(null));

    public SeccionTienda() => InitializeComponent();

    public StoreShopViewModel ViewModel
    {
        get => (StoreShopViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    // Cada botón trae su tarjeta en el DataContext
    private void OnBuyClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ShopEmoteViewModel item }) ViewModel.BuyCommand.Execute(item);
    }

    private void OnSearchKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != VirtualKey.Enter) return;
        e.Handled = true;
        ViewModel.SearchCommand.Execute(null);
    }
}
