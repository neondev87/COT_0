// Compras de un usuario del login en compras.json (sin lógica)
namespace COT_0.Host.Models;

/// <summary>Emotes comprados, tickets, desde cuándo corre su ventana de cupo y cuándo llegó al límite (UTC; null si no aplica).</summary>
public class UserPurchases
{
    public string UserName { get; set; } = string.Empty;
    public DateTime? WindowStart { get; set; }

    /// <summary>Hora UTC de la compra que llegó al límite: desde ahí corren las horas de espera (null si no ha llegado).</summary>
    public DateTime? LimitReachedAt { get; set; }
    public List<PurchasedEmote> Emotes { get; set; } = [];
    public List<PurchaseTicket> Tickets { get; set; } = [];
}
