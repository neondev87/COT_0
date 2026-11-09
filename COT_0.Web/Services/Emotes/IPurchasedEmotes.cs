using COT_0.Core.Models;

namespace COT_0.Web.Services.Emotes;

/// <summary>
/// Emotes comprados en la tienda del Host que se suman a los del set (misma instancia que IRoomEmotes).
/// Va en Web y no en Core porque Core solo lo cambia el Agente 0 (pedido anotado en CONTRATO.md)
/// </summary>
public interface IPurchasedEmotes
{
    IReadOnlyList<RoomEmote> Purchased { get; }

    // Reemplaza la lista completa (al abrir la sala o al comprar con la sala abierta); vacía los quita
    void SetPurchased(IReadOnlyList<RoomEmote> emotes);
}
