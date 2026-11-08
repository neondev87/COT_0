// Escucha el chat y anota en visitas.json a cada invitado nuevo que entra
using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Host.Services.Sala;

namespace COT_0.Host.Services.Visitas;

/// <summary>Bitácora general de entradas, sin importar la sesión.</summary>
public sealed class VisitWatcher(IChatService chat) : GuestWatcher(chat)
{
    protected override Task OnGuestJoinedAsync(User user) => VisitService.RecordAsync(user.Name);
}
