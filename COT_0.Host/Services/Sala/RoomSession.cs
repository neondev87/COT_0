// Ciclo de vida de la sala: servidor + túnel + sala, y las preguntas al salir o cerrar
using COT_0.Core.Services;
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Dialogos;
using COT_0.Host.Services.Sesiones;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Visitas;
using Microsoft.Extensions.DependencyInjection;

namespace COT_0.Host.Services.Sala;

/// <summary>
/// Abrir: servidor → vigilantes (visitas y sesiones) → túnel → emotes comprados → sala.
/// Cerrar: sala (y hora de fin de la sesión) → vigilantes → túnel.
/// Kestrel sigue corriendo con la sala cerrada (los ViewModels guardan sus IChatService/IRoomService);
/// se detiene al cerrar la app con ShutdownAsync.
/// </summary>
public class RoomSession(WebServerHost webServer, ITunnelService tunnel, ConfirmDialogService dialogs, ChatEmoteImport emoteImport)
{
    // Uno por sala abierta: se sueltan al cerrarla para no escuchar el chat de más
    private readonly List<IDisposable> _watchers = [];

    // Se pide hasta usarla: así el login no espera a que se arme la web
    private IRoomService Room => webServer.Services.GetRequiredService<IRoomService>();

    public bool IsActive => Room.IsOpen || tunnel.IsRunning;

    // URL pública del túnel o, si no hay, la local
    public string Url => tunnel.PublicUrl ?? webServer.Url;

    public bool IsServerRunning => webServer.IsRunning;

    public async Task OpenAsync()
    {
        try
        {
            await webServer.StartAsync();
            AttachWatchers();
            await tunnel.StartAsync(webServer.Url);
            // Los emotes comprados por quien hizo ENTRAR entran solos al chat
            await emoteImport.ImportAsync();
            Room.Open();
        }
        catch
        {
            ReleaseWatchers();
            throw;
        }
    }

    public async Task CloseAsync()
    {
        await CloseRoomAsync();
        emoteImport.Clear();
        ReleaseWatchers();
        await tunnel.StopAsync();
    }

    /// <summary>Al cerrar la app: cierra la sala si hay y apaga Kestrel.</summary>
    public async Task ShutdownAsync()
    {
        if (IsActive) await CloseAsync();
        await webServer.StopAsync();
    }

    /// <summary>SALIR: con sala activa "¿Cerrar la sala y salir?", sin sala "¿Cerrar sesión?"; true si confirmaste.</summary>
    public Task<bool> ConfirmLogoutAsync() => ConfirmAsync(IsActive ? "ExitDialog" : "LogoutDialog");

    /// <summary>✕ de la ventana: con sala activa "¿Cerrar la sala y salir?", sin sala "¿Salir de COT_0?".</summary>
    public Task<bool> ConfirmCloseAppAsync() => ConfirmAsync(IsActive ? "ExitDialog" : "CloseAppDialog");

    /// <summary>CERRAR SALA: "¿Cerrar la sala?"; true si confirmaste.</summary>
    public Task<bool> ConfirmCloseRoomAsync() => ConfirmAsync("CloseRoomDialog");

    // Cada diálogo tiene Text.{clave}.Message, .Confirm y .Cancel
    private Task<bool> ConfirmAsync(string dialogKey) => dialogs.ConfirmAsync(new ConfirmTexts(
        UiText.Get($"Text.{dialogKey}.Message"),
        UiText.Get($"Text.{dialogKey}.Confirm"),
        UiText.Get($"Text.{dialogKey}.Cancel")));

    // Saca a los invitados, vacía el chat y deja guardada la hora de fin
    private async Task CloseRoomAsync()
    {
        if (Room.IsOpen) await Room.CloseAsync();
        try
        {
            // SesionWatcher ya la cierra con IRoomService.Changed; esperarla aquí asegura que quede guardada
            await SesionService.EndAsync();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
        }
    }

    // Ya con la web arriba existen IChatService.UsersChanged e IRoomService.Changed
    private void AttachWatchers()
    {
        if (_watchers.Count > 0) return;

        var chat = webServer.Services.GetRequiredService<IChatService>();
        _watchers.Add(new VisitWatcher(chat));
        _watchers.Add(new SesionWatcher(chat, Room));
    }

    private void ReleaseWatchers()
    {
        foreach (var watcher in _watchers) watcher.Dispose();
        _watchers.Clear();
    }
}
