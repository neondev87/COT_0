// Cómo se dibuja un renglón del ticket en PDF
namespace COT_0.Host.Models;

/// <summary>Lo usa TicketDocument para elegir fuente y tamaño.</summary>
public enum TicketLineKind
{
    Title,
    Text,
    Bold,
    Rule,
}
