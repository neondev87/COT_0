using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Services;
using Microsoft.AspNetCore.SignalR;

namespace COT_0.Web.Hubs;

// Pasa los eventos de IChatService / IRoomService a los clientes del hub
// (también los que dispara el Host: expulsar, cerrar la sala)
public sealed class ChatBroadcaster(IChatService chat, IUserEditor editor, IRoomService room, IHubContext<ChatHub> hub, ILogger<ChatBroadcaster> logger)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        chat.UsersChanged += OnUsersChanged;
        chat.UserKicked += OnUserKicked;
        editor.UserUpdated += OnUserUpdated;
        room.Changed += OnRoomChanged;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        chat.UsersChanged -= OnUsersChanged;
        chat.UserKicked -= OnUserKicked;
        editor.UserUpdated -= OnUserUpdated;
        room.Changed -= OnRoomChanged;
        return Task.CompletedTask;
    }

    private void OnUsersChanged() => Fire(async () =>
        await hub.Clients.All.SendAsync(ChatEvents.UsersChanged, await chat.GetUsersAsync()));

    // Cada cliente reemplaza nombre, foto y escudo en los mensajes ya pintados de ese usuario
    private void OnUserUpdated(User user) => Fire(() => hub.Clients.All.SendAsync(ChatEvents.UserUpdated, user));

    private void OnUserKicked(User user)
    {
        if (user.ConnectionId is { } connectionId)
            Fire(() => hub.Clients.Client(connectionId).SendAsync(ChatEvents.Kicked));
    }

    private void OnRoomChanged()
    {
        if (!room.IsOpen) Fire(() => hub.Clients.All.SendAsync(ChatEvents.RoomClosed));
    }

    // Los eventos son síncronos: el envío sigue en segundo plano y se loguea si falla
    private void Fire(Func<Task> send) => _ = Task.Run(async () =>
    {
        try { await send(); }
        catch (Exception ex) { logger.LogWarning(ex, "No se pudo avisar a los clientes del chat"); }
    });
}
