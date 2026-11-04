using System.Threading.Channels;
using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Services;
using Microsoft.AspNetCore.SignalR;

namespace COT_0.Web.Hubs;

// Pasa los eventos de IChatService / IRoomService a los clientes del hub
// (también los que dispara el Host: expulsar, cerrar la sala)
public sealed class ChatBroadcaster(
    IChatService chat, IChatFeed feed, IUserEditor editor, IRoomService room, IHubContext<ChatHub> hub, ILogger<ChatBroadcaster> logger)
    : BackgroundService
{
    // Una sola cola: los avisos salen en el mismo orden en que pasaron
    private readonly Channel<Func<Task>> _queue = Channel.CreateUnbounded<Func<Task>>(new() { SingleReader = true });

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        chat.UsersChanged += OnUsersChanged;
        chat.UserKicked += OnUserKicked;
        feed.MessageAdded += OnMessageAdded;
        editor.UserUpdated += OnUserUpdated;
        room.Changed += OnRoomChanged;

        try
        {
            await foreach (var send in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try { await send(); }
                catch (Exception ex) { logger.LogWarning(ex, "No se pudo avisar a los clientes del chat"); }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            chat.UsersChanged -= OnUsersChanged;
            chat.UserKicked -= OnUserKicked;
            feed.MessageAdded -= OnMessageAdded;
            editor.UserUpdated -= OnUserUpdated;
            room.Changed -= OnRoomChanged;
        }
    }

    private void OnUsersChanged() => Enqueue(async () =>
        await hub.Clients.All.SendAsync(ChatEvents.UsersChanged, await chat.GetUsersAsync()));

    private void OnMessageAdded(ChatMessage message) => Enqueue(() => hub.Clients.All.SendAsync(ChatEvents.MessageReceived, message));

    // Cada cliente reemplaza nombre, foto y escudo en los mensajes ya pintados de ese usuario
    private void OnUserUpdated(User user) => Enqueue(() => hub.Clients.All.SendAsync(ChatEvents.UserUpdated, user));

    private void OnUserKicked(User user)
    {
        if (user.ConnectionId is { } connectionId)
            Enqueue(() => hub.Clients.Client(connectionId).SendAsync(ChatEvents.Kicked));
    }

    private void OnRoomChanged()
    {
        if (!room.IsOpen) Enqueue(() => hub.Clients.All.SendAsync(ChatEvents.RoomClosed));
    }

    private void Enqueue(Func<Task> send) => _queue.Writer.TryWrite(send);
}
