// Un renglón del ticket en PDF ya con su texto
namespace COT_0.Host.Models;

/// <summary>Kind dice cómo se dibuja: título grande, texto, texto en negritas o línea separadora.</summary>
public record TicketLine(string Text, TicketLineKind Kind = TicketLineKind.Text);
