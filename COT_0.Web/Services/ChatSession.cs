using System.Collections.Immutable;
using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Hubs;
using COT_0.Web.Models;
using COT_0.Web.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

public sealed class ChatSession(IChatService chat, IOptions<ChatOptions> options) : IChatSession
{
    private readonly ChatOptions _options = options.Value;
    private HubConnection? _connection;

    // Listas inmutables: el hub las reemplaza desde otro hilo mientras Blazor renderiza
    private ImmutableList<ChatMessage> _messages = [];

    public User? CurrentUser { get; private set; }
    public IReadOnlyList<ChatMessage> Messages => _messages;
    public IReadOnlyList<User> Users { get; private set; } = [];

    public event Action? Changed;
    public event Action<string>? Ended;

    public async Task<bool> StartAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId)) return false;

        CurrentUser = await chat.GetUserAsync(userId);
        if (CurrentUser is null) return false;

        _messages = [.. await chat.GetMessagesAsync(_options.MessagesOnJoin)];
        Users = await chat.GetUsersAsync();

        _connection = new HubConnectionBuilder().WithUrl(_options.HubUrl).Build();
        _connection.On<ChatMessage>(ChatEvents.MessageReceived, OnMessageReceived);
        _connection.On<List<User>>(ChatEvents.UsersChanged, OnUsersChanged);
        _connection.On<User>(ChatEvents.UserUpdated, OnUserUpdated);
        _connection.On(ChatEvents.Kicked, () => Ended?.Invoke(LeaveReason.Kicked));
        _connection.On(ChatEvents.RoomClosed, () => Ended?.Invoke(LeaveReason.RoomClosed));

        await _connection.StartAsync();
        await _connection.InvokeAsync(nameof(ChatHub.Join), userId);
        return true;
    }

    public async Task SendAsync(string text)
    {
        if (_connection is null || string.IsNullOrWhiteSpace(text)) return;
        await _connection.InvokeAsync(nameof(ChatHub.SendMessage), text);
    }

    public bool IsOwn(ChatMessage message) => message.UserId == CurrentUser?.Id;

    public async Task StopAsync()
    {
        if (_connection is null) return;

        await _connection.DisposeAsync();
        _connection = null;
    }

    public async ValueTask DisposeAsync() => await StopAsync();

    private void OnMessageReceived(ChatMessage message)
    {
        var messages = _messages.Add(message);
        _messages = messages.Count > _options.MessagesOnJoin ? messages.RemoveAt(0) : messages;
        Changed?.Invoke();
    }

    // Mismo Id → Blazor conserva cada elemento (@key) y el scroll no se mueve
    private void OnUserUpdated(User user)
    {
        _messages = _messages.ConvertAll(m => m.UserId == user.Id ? MessageAuthor.With(m, user) : m);
        Changed?.Invoke();
    }

    private void OnUsersChanged(List<User> users)
    {
        Users = users;
        Changed?.Invoke();
    }
}
