using System.Collections.Concurrent;
using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

// Estado del chat en memoria (Singleton)
public class InMemoryChatService(IOptions<ChatOptions> options) : IChatService
{
    private readonly ConcurrentDictionary<string, User> _users = new();
    private readonly List<ChatMessage> _messages = [];
    private readonly object _lock = new();

    public event Action? UsersChanged;
    public event Action<User>? UserKicked;

    public Task<User> AddUserAsync(User user)
    {
        _users[user.Id] = user;
        UsersChanged?.Invoke();
        return Task.FromResult(user);
    }

    public Task RemoveUserAsync(string userId)
    {
        if (_users.TryRemove(userId, out _)) UsersChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task KickUserAsync(string userId)
    {
        if (!_users.TryRemove(userId, out var user)) return Task.CompletedTask;
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

    public Task<ChatMessage> AddMessageAsync(string userId, string text)
    {
        if (!_users.TryGetValue(userId, out var user))
            throw new InvalidOperationException($"El usuario {userId} no está conectado.");

        var message = new ChatMessage
        {
            UserId = user.Id,
            UserName = user.Name,
            UserPhotoUrl = user.PhotoUrl,
            UserFacultyFlag = user.FacultyFlag,
            Text = text,
        };

        lock (_lock)
        {
            _messages.Add(message);
            if (_messages.Count > options.Value.HistorySize) _messages.RemoveAt(0);
        }

        return Task.FromResult(message);
    }

    public Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int max = 100)
    {
        lock (_lock)
        {
            return Task.FromResult<IReadOnlyList<ChatMessage>>(_messages.TakeLast(max).ToList());
        }
    }
}
