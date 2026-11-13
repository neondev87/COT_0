// Sección "Inputs" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>Largo máximo de todo lo que escribe el host, mínimo de la contraseña y los símbolos que acepta.</summary>
public record InputsSettings(int MaxCaracteres, int MinPasswordCaracteres, string PasswordSymbols);
