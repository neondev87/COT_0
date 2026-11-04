using System.ComponentModel.DataAnnotations;
using COT_0.Core.Models;

namespace COT_0.Web.Models;

// Datos del formulario de perfil (/join y editar perfil)
public class JoinForm
{
    [Required(ErrorMessage = "Elegí un nombre.")]
    public string Name { get; set; } = string.Empty;

    public string? PhotoUrl { get; set; }
    public string? FacultyFlag { get; set; }

    // Formulario ya lleno con un perfil existente
    public static JoinForm From(User user) =>
        new() { Name = user.Name, PhotoUrl = user.PhotoUrl, FacultyFlag = user.FacultyFlag };
}
