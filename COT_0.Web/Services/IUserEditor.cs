using COT_0.Core.Models;

namespace COT_0.Web.Services;

// Edición del perfil de un invitado conectado (dispara IChatService.UsersChanged y UserUpdated).
// Queda en Web hasta que Core sume UpdateUserAsync a IChatService (ver CONTRATO.md).
public interface IUserEditor
{
    // También actualiza la copia del autor en sus mensajes guardados
    Task<User?> UpdateUserAsync(string userId, string name, string? photoUrl, string? facultyFlag);

    event Action<User>? UserUpdated;
}
