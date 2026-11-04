using COT_0.Core.Services;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Ciclo de vida de la sala: servidor + túnel + sala, y la pregunta al salir con sala activa
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
        await CloseRoomAsync();
        await StopTunnelAsync();
    }

    // Saca a los invitados y vacía el chat
    public async Task CloseRoomAsync()
    {
        if (room.IsOpen) await room.CloseAsync();
    }

    public Task StopTunnelAsync() => tunnel.StopAsync();

    // "Hay una sala activa. ¿Cerrar la sala y salir?"; true si el host confirmó
    public bool ConfirmLeave() => dialogs.Confirm(
        configuration["Texts:ExitDialog:Message"] ?? string.Empty,
        configuration["Texts:ExitDialog:Confirm"] ?? string.Empty,
        configuration["Texts:ExitDialog:Cancel"] ?? string.Empty);
}
