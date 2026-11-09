// Ticket de una compra de la tienda tal como se guarda en compras.json (sin lógica)
namespace COT_0.Host.Models;

/// <summary>Número consecutivo (único entre todos los usuarios, así no chocan los PDF), fecha UTC, emotes y cupo que quedó.</summary>
public class PurchaseTicket
{
    public int Number { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PurchasedEmote> Emotes { get; set; } = [];
    public int Remaining { get; set; }
    public DateTime ResetsAt { get; set; }
}
