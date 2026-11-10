using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Foundation;
using Windows.System;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>
/// Sección TIENDA: la página le pasa su StoreShopViewModel; los botones y ENTER solo llaman a sus comandos.
/// Al comprar, solo anima: coloca la copia del emote donde estaba (tarjeta o carrito) y la manda al contador.
/// </summary>
public sealed partial class SeccionTienda : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(StoreShopViewModel), typeof(SeccionTienda), new PropertyMetadata(null, OnViewModelChanged));

    public SeccionTienda() => InitializeComponent();

    public StoreShopViewModel ViewModel
    {
        get => (StoreShopViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var view = (SeccionTienda)d;
        if (e.OldValue is StoreShopViewModel old) old.Bought -= view.PlayBuyEffect;
        if (e.NewValue is StoreShopViewModel vm) vm.Bought += view.PlayBuyEffect;
    }

    // Cada botón trae su tarjeta en el DataContext
    private void OnPickClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ShopEmoteViewModel item }) ViewModel.PickCommand.Execute(item);
    }

    private void OnRemoveClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: ShopEmoteViewModel item }) ViewModel.RemoveFromCartCommand.Execute(item);
    }

    private void OnSearchKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != VirtualKey.Enter) return;
        e.Handled = true;
        ViewModel.SearchCommand.Execute(null);
    }

    private void PlayBuyEffect(IReadOnlyList<ShopEmoteViewModel> items, bool fromCart)
    {
        if (items.Count == 0) return;
        FrameworkElement source = fromCart ? CartPanel : Grid7tv.ContainerFromItem(items[0]) as FrameworkElement ?? Grid7tv;
        var from = Center(source);
        var to = Center(QuotaCounter);
        var half = Ghost.Width / 2;
        Canvas.SetLeft(Ghost, from.X - half);
        Canvas.SetTop(Ghost, from.Y - half);
        Ghost.Source = items[0].Thumbnail;

        var effect = (Storyboard)Resources["BuyEffect"];
        effect.Stop();
        ((DoubleAnimation)effect.Children[0]).To = to.X - from.X;
        ((DoubleAnimation)effect.Children[1]).To = to.Y - from.Y;
        effect.Begin();
    }

    private Point Center(FrameworkElement element) =>
        element.TransformToVisual(Root).TransformPoint(new Point(element.ActualWidth / 2, element.ActualHeight / 2));
}
