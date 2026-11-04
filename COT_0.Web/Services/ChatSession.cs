using System.Collections.Immutable;
using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Hubs;
using COT_0.Web.Models;
using COT_0.Web.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

public sealed class ChatSession(IChatService chat, IOptions<ChatOptions> options, ILogger<ChatSession> logger) : IChatSession
{
    private readonly ChatOptions _options = options.Value;
    private readonly TypingTracker _typing = new(TimeSpan.FromSeconds(options.Value.TypingTimeoutSeconds));
    private HubConnection? _connection;
    private DateTime _lastTypingSent;

    // Listas inmutables: el hub las reemplaza desde otro hilo mientras Blazor renderiza
    private ImmutableList<ChatMessage> _messages = [];

    public User? CurrentUser { get; private set; }
    public IReadOnlyList<MessageGroup> Groups { get; private set; } = [];
    public IReadOnlyList<User> Users { get; private set; } = [];
    public string? TypingSummary => _typing.Summary;

    public event Action? Changed;
    public event Action<string?>? Ended;

    public async Task<bool> StartAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId)) return false;

        CurrentUser = await chat.GetUserAsync(userId);
        if (CurrentUser is null) return false;

        _typing.Changed += RaiseChanged;
        _connection = new HubConnectionBuilder().WithUrl(_options.HubUrl).WithAutomaticReconnect().Build();
        _connection.On<ChatMessage>(ChatEvents.MessageReceived, OnMessageReceived);
        _connection.On<List<User>>(ChatEvents.UsersChanged, OnUsersChanged);
        _connection.On<User>(ChatEvents.UserUpdated, OnUserUpdated);
        _connection.On<string, string>(ChatEvents.UserTyping, OnUserTyping);
        _connection.On(ChatEvents.Kicked, () => Ended?.Invoke(LeaveReason.Kicked));
        _connection.On(ChatEvents.RoomClosed, () => Ended?.Invoke(LeaveReason.RoomClosed));
        _connection.Reconnected += _ => RejoinAsync();
        _connection.Closed += OnClosed;

        await _connection.StartAsync();
        await JoinAndSyncAsync();
        return true;
    }

    public async Task SendAsync(string text)
    {
        if (_connection?.State != HubConnectionState.Connected || string.IsNullOrWhiteSpace(text)) return;

        _lastTypingSent = DateTime.MinValue;
        await _connection.InvokeAsync(nameof(ChatHub.SendMessage), text);
    }

    // Como mucho un aviso cada Chat:TypingThrottleSeconds
    public async Task NotifyTypingAsync()
    {
        if (_connection?.State != HubConnectionState.Connected) return;
        if (DateTime.UtcNow - _lastTypingSent < TimeSpan.FromSeconds(_options.TypingThrottleSeconds)) return;

        _lastTypingSent = DateTime.UtcNow;
        await _connection.SendAsync(nameof(ChatHub.Typing));
    }

    public async Task StopAsync()
    {
        _typing.Changed -= RaiseChanged;
        _typing.Dispose();
        if (_connection is null) return;

        await _connection.DisposeAsync();
        _connection = null;
    }

    public async ValueTask DisposeAsync() => await StopAsync();

    // Se une con la conexión actual y trae del servidor lo que se haya perdido
    private async Task JoinAndSyncAsync()
    {
        await _connection!.InvokeAsync(nameof(ChatHub.Join), CurrentUser!.Id);
        SetMessages([.. await chat.GetMessagesAsync(_options.MessagesOnJoin)]);
        Users = await chat.GetUsersAsync();
        RaiseChanged();
    }

    private async Task RejoinAsync()
    {
        try
        {
            await JoinAndSyncAsync();
        }
        catch (Exception ex)
        {
            // Ya lo sacaron de la sala mientras estaba desconectado: vuelve a entrar con el perfil guardado
            logger.LogInformation(ex, "No se pudo volver a unir al chat");
            Ended?.Invoke(null);
        }
    }

    private void OnMessageReceived(ChatMessage message)
    {
        // Puede llegar repetido si coincide con la resincronización
        if (_messages.Exists(m => m.Id == message.Id)) return;

        var messages = _messages.Add(message);
        SetMessages(messages.Count > _options.MessagesOnJoin ? messages.RemoveAt(0) : messages);
        _typing.Clear(message.UserId);
        RaiseChanged();
    }

    // Mismo Id → Blazor conserva cada elemento (@key) y el scroll no se mueve
    private void OnUserUpdated(User user)
    {
        SetMessages(_messages.ConvertAll(m => m.UserId == user.Id ? MessageAuthor.With(m, user) : m));
        RaiseChanged();
    }

    private void OnUsersChanged(List<User> users)
    {
        Users = users;
        RaiseChanged();
    }

    private void OnUserTyping(string userId, string name)
    {
        if (userId != CurrentUser?.Id) _typing.Mark(userId, name);
    }

    private void SetMessages(ImmutableList<ChatMessage> messages)
    {
        _messages = messages;
        Groups = MessageGrouping.Group(messages, CurrentUser?.Id, TimeSpan.FromMinutes(_options.GroupWindowMinutes));
    }

    // Se cortó del todo (no por StopAsync): vuelve a entrar con el perfil guardado
    private Task OnClosed(Exception? error)
    {
        if (error is not null) Ended?.Invoke(null);
        return Task.CompletedTask;
    }

    private void RaiseChanged() => Changed?.Invoke();
}
