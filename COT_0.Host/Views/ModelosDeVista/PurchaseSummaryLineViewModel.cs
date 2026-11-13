// Un renglón del panel COMPRA REALIZADA: miniatura, nombre y precio de muestra
using Microsoft.UI.Xaml.Media.Imaging;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Reusa la miniatura que ya tenía la tarjeta de la tienda (no se vuelve a descargar).</summary>
public sealed class PurchaseSummaryLineViewModel(string name, string priceText, BitmapImage thumbnail)
{
    public string Name { get; } = name;

    public string PriceText { get; } = priceText;

    public BitmapImage Thumbnail { get; } = thumbnail;
}
