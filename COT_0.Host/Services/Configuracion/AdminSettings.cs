// Sección "Admin" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>
/// Cuántos administradores caben, largo de la contraseña que genera la app, sus letras y números
/// (sin caracteres ambiguos como 0/O o l/1; los símbolos salen de Inputs:PasswordSymbols)
/// y a los cuántos segundos se limpia el portapapeles si sigue teniendo esa contraseña.
/// </summary>
public record AdminSettings(
    int Maximo,
    int PasswordLength,
    string PasswordUpper,
    string PasswordLower,
    string PasswordDigits,
    int ClipboardClearSeconds);
