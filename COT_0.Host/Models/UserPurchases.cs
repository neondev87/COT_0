// Compras de un usuario del login en compras.json (sin lógica)
namespace COT_0.Host.Models;

/// <summary>Emotes comprados, tickets y desde cuándo corre su ventana de cupo (UTC; null si no hay ventana abierta).</summary>
public class UserPurchases
{
    public string UserName { get; set; } = string.Empty;
    public DateTime? WindowStart { get; set; }
    public List<PurchasedEmote> Emotes { get; set; } = [];
    public List<PurchaseTicket> Tickets { get; set; } = [];
}
