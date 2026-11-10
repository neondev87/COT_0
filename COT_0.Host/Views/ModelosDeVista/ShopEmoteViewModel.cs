// Tarjeta de un emote en la TIENDA: imagen, nombre y AGREGAR (carrito) o COMPRAR (uno por uno); COMPRADO si ya es tuyo
using COT_0.Core.Models;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>CanBuy lo apaga la tienda cuando se acaba el cupo, el carrito está lleno o mientras se guarda una compra.</summary>
public partial class ShopEmoteViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonText), nameof(IsBuyEnabled))]
    private bool _isOwned;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBuyEnabled))]
    private bool _canBuy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonText), nameof(IsBuyEnabled))]
    private bool _isInCart;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonText))]
    private bool _isCartMode;

    public ShopEmoteViewModel(RoomEmote emote, bool isOwned, bool canBuy, bool isCartMode)
    {
        Emote = emote;
        _isOwned = isOwned;
        _canBuy = canBuy;
        _isCartMode = isCartMode;
        Thumbnail = EmoteThumbViewModel.Create(EmoteMapping.ThumbnailUrl(emote));
    }

    public RoomEmote Emote { get; }

    public string Name => Emote.Name;

    public BitmapImage Thumbnail { get; }

    public bool IsBuyEnabled => CanBuy && !IsOwned && !IsInCart;

    public string ButtonText => UiText.Get(
        IsOwned ? "Text.Store.Shop.Owned"
        : IsInCart ? "Text.Store.Shop.InCart"
        : IsCartMode ? "Text.Store.Shop.Add"
        : "Text.Store.Shop.Buy");
}
