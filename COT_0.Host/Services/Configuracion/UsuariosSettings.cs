// Sección "Usuarios" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>Cuántos usuarios se pueden registrar en el login de esta computadora.</summary>
public record UsuariosSettings(int Maximo);
