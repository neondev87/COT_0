// Ciclo de vida de la sala: servidor + túnel + sala, y la pregunta al salir con sala activa
using COT_0.Core.Services;
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Dialogos;
using COT_0.HostWinUI.Services.Textos;
using Microsoft.Extensions.DependencyInjection;

namespace COT_0.HostWinUI.Services.Sala;

public class RoomSession(WebServerHost webServer, ITunnelService tunnel, ConfirmDialogService dialogs)
{
    // Se pide hasta usarla: así el login no espera a que se arme la web
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

    /// <summary>"Hay una sala activa. ¿Cerrar la sala y salir?"; true si confirmaste.</summary>
    public Task<bool> ConfirmLeaveAsync() => dialogs.ConfirmAsync(new ConfirmTexts(
        UiText.Get("Text.ExitDialog.Message"),
        UiText.Get("Text.ExitDialog.Confirm"),
        UiText.Get("Text.ExitDialog.Cancel")));
}
