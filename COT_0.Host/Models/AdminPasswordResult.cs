// Resultado de crear un admin o regenerar su contraseña
namespace COT_0.Host.Models;

/// <summary>Password es la contraseña generada, en claro solo para mostrarla una vez (null si hubo error).</summary>
public record AdminPasswordResult(AdminError Error, string? Password)
{
    public static AdminPasswordResult Fail(AdminError error) => new(error, null);
}
