using COT_0.Core.Models;

namespace COT_0.Web.Services;

// Edición del perfil de un invitado conectado (dispara IChatService.UsersChanged).
// Queda en Web hasta que Core sume UpdateUserAsync a IChatService (ver CONTRATO.md).
public interface IUserEditor
{
    Task<User?> UpdateUserAsync(string userId, string name, string? photoUrl, string? facultyFlag);
}
