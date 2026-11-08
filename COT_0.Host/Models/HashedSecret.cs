// Resultado de hashear una contraseña o una respuesta secreta
namespace COT_0.Host.Models;

/// <summary>Hash y salt en Base64, listos para guardar en JSON.</summary>
public record HashedSecret(string Hash, string Salt);
