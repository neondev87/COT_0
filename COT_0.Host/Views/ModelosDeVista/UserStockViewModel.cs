// Recuadro con el stock de un usuario: sus emotes comprados, cuántos lleva hoy y cuántos le quedan
using COT_0.Host.Models;
using COT_0.Host.Services.Tienda;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Se arma en el hilo de la UI con lo leído de compras.json; si no compró nada, IsEmpty muestra el aviso.</summary>
public sealed class UserStockViewModel
{
    public UserStockViewModel(UserCardViewModel card, UserPurchases purchases, QuotaInfo quota)
    {
        Card = card;
        Title = StoreTexts.Format("Text.Store.Stock.Title", card.Name.ToUpperInvariant());
        Summary = StoreTexts.Format("Text.Store.Stock.Summary", purchases.Emotes.Count, quota.Used, quota.Remaining, quota.Limit);
        Emotes = purchases.Emotes
            .OrderByDescending(e => e.PurchasedAt)
            .Select(e => new EmoteThumbViewModel(e.Name, EmoteMapping.ThumbnailUrl(e)))
            .ToList();
    }

    public UserCardViewModel Card { get; }

    public string Title { get; }

    public string Summary { get; }

    public IReadOnlyList<EmoteThumbViewModel> Emotes { get; }

    public bool IsEmpty => Emotes.Count == 0;

    public bool HasEmotes => !IsEmpty;
}
