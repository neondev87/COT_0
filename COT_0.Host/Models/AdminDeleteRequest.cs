// Lo que se escribe para eliminar a un admin: la respuesta secreta del que se va y la contraseña de quien elimina
namespace COT_0.Host.Models;

/// <summary>Vive solo mientras se revisa; nunca se guarda ni se muestra.</summary>
public record AdminDeleteRequest(string TargetAnswer, string ActorPassword);
