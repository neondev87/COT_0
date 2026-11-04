using COT_0.Core.Services;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Ciclo de vida de la sala: servidor + túnel + sala, y confirmación al salir con sala activa
public class RoomSession(WebServerHost webServer, ITunnelService tunnel, IRoomService room,
    DialogService dialogs, IConfiguration configuration)
{
    public bool IsActive => room.IsOpen || tunnel.IsRunning;

    public async Task OpenAsync()
    {
        await webServer.StartAsync();
        await tunnel.StartAsync(WebServerHost.Url);
        room.Open();
    }

    public async Task CloseAsync()
    {
        if (room.IsOpen) await room.CloseAsync();
        await tunnel.StopAsync();
    }

    // true si se puede salir: no hay sala activa, o el host confirmó y ya se cerró
    public async Task<bool> ConfirmCloseAsync()
    {
        if (!IsActive) return true;

        var confirmed = dialogs.Confirm(
            configuration["Texts:ExitDialog:Message"] ?? string.Empty,
            configuration["Texts:ExitDialog:Confirm"] ?? string.Empty,
            configuration["Texts:ExitDialog:Cancel"] ?? string.Empty);
        if (!confirmed) return false;

        await CloseAsync();
        return true;
    }
}
