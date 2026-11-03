using COT_0.Core.Models;

namespace COT_0.Core.Services;

// Estado del chat en memoria: usuarios conectados e historial de mensajes
public interface IChatService
{
    // Se dispara cuando entra, sale o es expulsado un usuario, o se vacía la sala
    event Action? UsersChanged;

    // Se dispara al expulsar a un usuario (el hub le avisa a su conexión)
    event Action<User>? UserKicked;

    Task<User> AddUserAsync(User user);
    Task RemoveUserAsync(string userId);
    Task<User?> GetUserAsync(string userId);
    Task<IReadOnlyList<User>> GetUsersAsync();

    // Saca al usuario y dispara UserKicked
    Task KickUserAsync(string userId);

    // Borra usuarios e historial (al cerrar la sala)
    Task ClearAsync();

    Task<ChatMessage> AddMessageAsync(string userId, string text);
    Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int max = 100);
}
