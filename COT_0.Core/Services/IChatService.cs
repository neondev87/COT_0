using COT_0.Core.Models;

namespace COT_0.Core.Services;

/// <summary>Estado del chat en memoria: usuarios conectados e historial de mensajes</summary>
public interface IChatService
{
    // Se dispara cuando entra, sale o es expulsado un usuario, o se vacía la sala
    event Action? UsersChanged;

    // Se dispara al expulsar a un usuario (el hub le avisa a su conexión)
    event Action<User>? UserKicked;

    // Se dispara al editar un perfil (el hub lo reenvía para actualizar los mensajes ya pintados)
    event Action<User>? UserUpdated;

    Task<User> AddUserAsync(User user);
    Task RemoveUserAsync(string userId);
    Task<User?> GetUserAsync(string userId);

    // Cambia nombre, foto y escudo; también la copia del autor en sus mensajes guardados. null si ya no está
    Task<User?> UpdateUserAsync(string userId, string name, string? photoUrl, string? facultyFlag);

    Task<IReadOnlyList<User>> GetUsersAsync();

    // Saca al usuario y dispara UserKicked
    Task KickUserAsync(string userId);

    // Borra usuarios e historial (al cerrar la sala)
    Task ClearAsync();

    Task<ChatMessage> AddMessageAsync(string userId, string text);

    // Nota de voz (Kind = Voice → AudioUrl) o imagen (Kind = Image → ImageUrl)
    Task<ChatMessage> AddMediaAsync(string userId, string text, MessageKind kind, string url);

    Task<IReadOnlyList<ChatMessage>> GetMessagesAsync(int max = 100);
}
