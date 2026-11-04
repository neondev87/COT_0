using COT_0.Core.Models;
using COT_0.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace COT_0.Web.Services;

// Alta y edición del perfil del invitado (foto + datos de perfil)
public interface IProfileService
{
    Task<OperationResult<string>> SavePhotoAsync(IBrowserFile file);
    Task<OperationResult<User>> CreateUserAsync(JoinForm form);

    // Cambia nombre, foto y facultad, avisa a todos y actualiza localStorage
    Task<OperationResult<User>> UpdateUserAsync(string userId, JoinForm form);

    // Vuelve a entrar con el perfil guardado si es de la sala abierta; null = pedir el formulario.
    // Con leaveReason (expulsado, sala cerrada) borra el perfil guardado y no entra.
    Task<User?> ResumeAsync(string? leaveReason);
}
