// Entrada de un invitado (visitas.json y lista de invitados de cada sesión)
namespace COT_0.Host.Models;

/// <summary>Quién entró y a qué hora (local).</summary>
public class Visit
{
    public string Name { get; set; } = string.Empty;
    public DateTime EnteredAt { get; set; }
}
