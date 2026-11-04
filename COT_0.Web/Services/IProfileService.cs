using COT_0.Core.Models;
using COT_0.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace COT_0.Web.Services;

// Alta del invitado desde /join (foto + datos de perfil)
public interface IProfileService
{
    Task<OperationResult<string>> SavePhotoAsync(IBrowserFile file);
    Task<OperationResult<User>> CreateUserAsync(JoinForm form);

    // Vuelve a entrar con el perfil guardado si es de la sala abierta; null = pedir el formulario.
    // Con leaveReason (expulsado, sala cerrada) borra el perfil guardado y no entra.
    Task<User?> ResumeAsync(string? leaveReason);
}
