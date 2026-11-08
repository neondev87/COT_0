using System.Collections.Concurrent;
using COT_0.Core.Models;
using COT_0.Core.Services;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Sala;

// Estado del chat en memoria (Singleton)
public class InMemoryChatService(IOptions<ChatOptions> options) : IChatService, IChatFeed
{
    private readonly ChatOptions _options = options.Value;
    private readonly ConcurrentDictionary<string, User> _users = new();
    private readonly List<ChatMessage> _messages = [];
    private readonly object _lock = new();

    public event Action? UsersChanged;
    public event Action<User>? UserKicked;
    public event Action<User>? UserUpdated;
    public event Action<ChatMessage>? MessageAdded;

    public Task<User> AddUserAsync(User user)
    {
        _users[user.Id] = user;
        Store(SystemNotice.Create(MessageKind.Joined, _options.JoinedNotice, user));
        UsersChanged?.Invoke();
        return Task.FromResult(user);
    }

    public Task<User?> UpdateUserAsync(string userId, string name, string? photoUrl, string? facultyFlag)
    {
        if (!_users.TryGetValue(userId, out var user)) return Task.FromResult<User?>(null);

        lock (_lock)
        {
            user.Name = name;
            user.PhotoUrl = photoUrl;
            user.FacultyFlag = facultyFlag;
            foreach (var message in _messages.Where(m => m.UserId == userId)) MessageAuthor.Apply(message, user);
        }

        UserUpdated?.Invoke(user);
        UsersChanged?.Invoke();
        return Task.FromResult<User?>(user);
    }

    public Task RemoveUserAsync(string userId)
    {
        if (!_users.TryRemove(userId, out var user)) return Task.CompletedTask;

        Store(SystemNotice.Create(MessageKind.Left, _options.LeftNotice, user));
        UsersChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task KickUserAsync(string userId)
    {
        if (!_users.TryRemove(userId, out var user)) return Task.CompletedTask;

        Store(SystemNotice.Create(MessageKind.Kicked, _options.KickedNotice, user));
        UserKicked?.Invoke(user);
        UsersChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        _users.Clear();
        lock (_lock) _messages.Clear();
        UsersChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task<User?> GetUserAsync(string userId) =>
        Task.FromResult(_users.TryGetValue(userId, out var user) ? user : null);

    public Task<IReadOnlyList<User>> GetUsersAsync() =>
        Task.FromResult<IReadOnlyList<User>>(_users.Values.OrderBy(u => u.JoinedAt).ToList());

    public Task<ChatMessage> AddMessageAsync(string userId, string text) =>
        Task.FromResult(Add(userId, new ChatMessage { Text = text }));

    public Task<ChatMessage> AddMediaAsync(string userId, string text, MessageKind kind, string url) =>
        Task.FromResult(Add(userId, new ChatMessage
        {
            Text = text,
            Kind = kind,
            AudioUrl = kind == MessageKind.Voice ? url : null,
            ImageUrl = kind == MessageKind.Image ? url : null,
        }));

    public Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int max = 100)
    {
        lock (_lock)
        {
            return Task.FromResult<IReadOnlyList<ChatMessage>>(_messages.TakeLast(max).ToList());
        }
    }

    private ChatMessage Add(string userId, ChatMessage message)
    {
        if (!_users.TryGetValue(userId, out var user))
            throw new InvalidOperationException($"El usuario {userId} no está conectado.");

        message.UserId = user.Id;
        MessageAuthor.Apply(message, user);
        Store(message);
        return message;
    }

    // Guarda y avisa dentro del lock: así los mensajes salen en el mismo orden en que se guardaron
    private void Store(ChatMessage message)
    {
        lock (_lock)
        {
            _messages.Add(message);
            if (_messages.Count > _options.HistorySize) _messages.RemoveAt(0);
            MessageAdded?.Invoke(message);
        }
    }
}
