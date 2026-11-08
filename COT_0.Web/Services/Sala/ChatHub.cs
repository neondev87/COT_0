using COT_0.Core.Models;
using COT_0.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Sala;

/// <summary>Hub del chat grupal, mapeado en Chat:HubPath (/chathub). Los mensajes, la lista de usuarios y los avisos de expulsión los manda ChatBroadcaster.</summary>
public class ChatHub(IChatService chat, IPresenceTracker presence, IOptions<ChatOptions> options) : Hub
{
    private const string UserIdKey = "userId";

    // También se llama al reconectar: la conexión nueva pasa a ser la del usuario
    public async Task Join(string userId)
    {
        var user = await chat.GetUserAsync(userId) ?? throw new HubException("El usuario no existe.");

        user.ConnectionId = Context.ConnectionId;
        Context.Items[UserIdKey] = userId;
    }

    public async Task SendMessage(string text)
    {
        var user = await GetCurrentUserAsync();

        var clean = text.Trim();
        // Vacío o con el texto reservado de notas de voz o imágenes (solo entran por IVoiceService / IImageService)
        if (clean.Length == 0 || VoiceNote.IsVoice(clean) || ImageNote.IsImage(clean)) return;
        if (clean.Length > options.Value.MaxMessageLength) clean = clean[..options.Value.MaxMessageLength];

        await chat.AddMessageAsync(user.Id, clean);
    }

    public async Task Typing()
    {
        var user = await GetCurrentUserAsync();
        await Clients.Others.SendAsync(ChatEvents.UserTyping, user.Id, user.Name);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items.TryGetValue(UserIdKey, out var value) && value is string userId)
            presence.ScheduleLeave(userId, Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    private async Task<User> GetCurrentUserAsync()
    {
        if (!Context.Items.TryGetValue(UserIdKey, out var value) || value is not string userId)
            throw new HubException("Primero hay que unirse al chat.");

        return await chat.GetUserAsync(userId) ?? throw new HubException("Ya no estás en la sala.");
    }
}
