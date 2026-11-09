// Tarjeta de un emote en la TIENDA: imagen, nombre y COMPRAR (o COMPRADO si ya es tuyo)
using COT_0.Core.Models;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>CanBuy lo apaga la tienda cuando se acaba el cupo o mientras se guarda una compra.</summary>
public partial class ShopEmoteViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonText), nameof(IsBuyEnabled))]
    private bool _isOwned;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBuyEnabled))]
    private bool _canBuy;

    public ShopEmoteViewModel(RoomEmote emote, bool isOwned, bool canBuy)
    {
        Emote = emote;
        _isOwned = isOwned;
        _canBuy = canBuy;
        Thumbnail = EmoteThumbViewModel.Create(EmoteMapping.ThumbnailUrl(emote));
    }

    public RoomEmote Emote { get; }

    public string Name => Emote.Name;

    public BitmapImage Thumbnail { get; }

    public bool IsBuyEnabled => CanBuy && !IsOwned;

    public string ButtonText => UiText.Get(IsOwned ? "Text.Store.Shop.Owned" : "Text.Store.Shop.Buy");
}
