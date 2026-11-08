// Sección "Security" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>Parámetros de PBKDF2 (SHA-256) para contraseña y respuesta secreta.</summary>
public record SecuritySettings(int SaltBytes, int HashBytes, int HashIterations);
