// Cupo de compras de un usuario calculado en este momento
namespace COT_0.Host.Models;

/// <summary>Cuántos lleva en su ventana, cuántos le quedan y cuándo se reinicia (UTC; null si no ha comprado en esta ventana).</summary>
public record QuotaInfo(int Used, int Remaining, int Limit, DateTime? ResetsAt)
{
    public bool IsBlocked => Remaining <= 0;
}
