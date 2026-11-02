using System.ComponentModel.DataAnnotations;

namespace COT_0.Web.Models;

// Datos del formulario de /join
public class JoinForm
{
    [Required(ErrorMessage = "Elegí un nombre.")]
    public string Name { get; set; } = string.Empty;

    public string? PhotoUrl { get; set; }
    public string? FacultyFlag { get; set; }
}
