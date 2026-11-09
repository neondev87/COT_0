// Contenido completo de compras.json (sin lógica)
namespace COT_0.Host.Models;

/// <summary>Último número de ticket entregado y las compras de cada usuario.</summary>
public class PurchasesData
{
    public int LastTicketNumber { get; set; }
    public List<UserPurchases> Users { get; set; } = [];
}
