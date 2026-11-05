using COT_0.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace COT_0.Host.Services;

// Ciclo de vida de la sala: servidor + túnel + sala, y la pregunta al salir con sala activa
public class RoomSession(WebServerHost webServer, ITunnelService tunnel,
    DialogService dialogs, IConfiguration configuration)
{
    // Se pide recién al usarla: así el login no espera a que se arme la web
    private IRoomService Room => webServer.Services.GetRequiredService<IRoomService>();

    public bool IsActive => Room.IsOpen || tunnel.IsRunning;

    // URL pública del túnel o, si no hay, la local
    public string Url => tunnel.PublicUrl ?? webServer.Url;

    public async Task OpenAsync()
    {
        await webServer.StartAsync();
        await tunnel.StartAsync(webServer.Url);
        Room.Open();
    }

    public async Task CloseAsync()
    {
        await CloseRoomAsync();
        await StopTunnelAsync();
    }

    // Saca a los invitados y vacía el chat
    public async Task CloseRoomAsync()
    {
        if (Room.IsOpen) await Room.CloseAsync();
    }

    public Task StopTunnelAsync() => tunnel.StopAsync();

    // "Hay una sala activa. ¿Cerrar la sala y salir?"; true si el host confirmó
    public bool ConfirmLeave() => dialogs.Confirm(
        configuration.GetText("ExitDialog:Message"),
        configuration.GetText("ExitDialog:Confirm"),
        configuration.GetText("ExitDialog:Cancel"));
}
