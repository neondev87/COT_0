using COT_0.Core.Models;

namespace COT_0.Core.Services;

// Estado del chat en memoria: usuarios conectados e historial de mensajes
public interface IChatService
{
    Task<User> AddUserAsync(User user);
    Task RemoveUserAsync(string userId);
    Task<User?> GetUserAsync(string userId);
    Task<IReadOnlyList<User>> GetUsersAsync();

    Task<ChatMessage> AddMessageAsync(string userId, string text);
    Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int max = 100);
}
