// Lo que regresa una compra: resultado, cupo que quedó y su ticket (null si no se compró)
namespace COT_0.Host.Models;

/// <summary>La tienda lo usa para el aviso, el contador y la pregunta del PDF.</summary>
public record PurchaseResult(PurchaseOutcome Outcome, QuotaInfo Quota, PurchaseTicket? Ticket);
