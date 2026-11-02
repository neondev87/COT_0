using COT_0.Core.Services;
using COT_0.Web.Options;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Hubs;

// Hub del chat grupal, mapeado en Chat:HubPath (/chathub)
public class ChatHub(IChatService chat, IOptions<ChatOptions> options) : Hub
{
    private const string UserIdKey = "userId";

    public async Task Join(string userId)
    {
        var user = await chat.GetUserAsync(userId) ?? throw new HubException("El usuario no existe.");

        user.ConnectionId = Context.ConnectionId;
        Context.Items[UserIdKey] = userId;
        await BroadcastUsersAsync();
    }

    public async Task SendMessage(string text)
    {
        if (!Context.Items.TryGetValue(UserIdKey, out var value) || value is not string userId)
            throw new HubException("Primero hay que unirse al chat.");

        var clean = text.Trim();
        if (clean.Length == 0) return;
        if (clean.Length > options.Value.MaxMessageLength) clean = clean[..options.Value.MaxMessageLength];

        var message = await chat.AddMessageAsync(userId, clean);
        await Clients.All.SendAsync(ChatEvents.MessageReceived, message);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items.TryGetValue(UserIdKey, out var value) && value is string userId)
        {
            // Solo si sigue siendo su conexión actual (puede haber recargado la página)
            var user = await chat.GetUserAsync(userId);
            if (user?.ConnectionId == Context.ConnectionId)
            {
                await chat.RemoveUserAsync(userId);
                await BroadcastUsersAsync();
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task BroadcastUsersAsync() =>
        await Clients.All.SendAsync(ChatEvents.UsersChanged, await chat.GetUsersAsync());
}
