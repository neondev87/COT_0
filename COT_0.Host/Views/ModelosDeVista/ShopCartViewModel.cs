// Carrito de la TIENDA: emotes agregados (hasta lo que te queda de cupo), QUITAR, el contador y el total de muestra del panel lateral
using System.Collections.ObjectModel;
using COT_0.Host.Services.Tienda;
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Solo en memoria mientras estás en la tienda; COMPRAR lo vacía al terminar bien la compra.</summary>
public partial class ShopCartViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFull), nameof(CountText))]
    private int _capacity;

    public ShopCartViewModel() => Items.CollectionChanged += (_, _) =>
    {
        OnPropertyChanged(nameof(IsFull));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(HasItems));
        OnPropertyChanged(nameof(CountText));
        OnPropertyChanged(nameof(TotalText));
    };

    public ObservableCollection<ShopEmoteViewModel> Items { get; } = [];

    public bool IsFull => Items.Count >= Capacity;

    public bool IsEmpty => Items.Count == 0;

    public bool HasItems => !IsEmpty;

    public string CountText => StoreTexts.Format("Text.Store.Cart.Count", Items.Count, Capacity);

    public string TotalText => StoreTexts.Format("Text.Store.Cart.Total", EmotePricing.Format(Items.Sum(i => i.Price)));

    /// <summary>false si ya está o ya no cabe.</summary>
    public bool Add(ShopEmoteViewModel item)
    {
        if (IsFull || Items.Contains(item)) return false;
        Items.Add(item);
        item.IsInCart = true;
        return true;
    }

    public void Remove(ShopEmoteViewModel item)
    {
        Items.Remove(item);
        item.IsInCart = false;
    }

    public void Clear()
    {
        foreach (var item in Items) item.IsInCart = false;
        Items.Clear();
    }
}
