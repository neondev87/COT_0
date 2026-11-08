// Una sesión de invitados en sesiones.json: se abre con la sala (QR) y se cierra con ella
namespace COT_0.HostWinUI.Models;

/// <summary>Sesiones numeradas 1, 2, 3...; el host no cuenta como invitado.</summary>
public class SesionInvitados
{
    public int Number { get; set; }
    public DateTime StartedAt { get; set; }

    /// <summary>null mientras la sala sigue abierta (o si la app se cerró de golpe).</summary>
    public DateTime? EndedAt { get; set; }

    public List<Visit> Guests { get; set; } = [];
}
