// Sección "Web" de appsettings.json (la misma que lee COT_0.Web)
namespace COT_0.Host.Services.Configuracion;

/// <summary>URL local del servidor y ruta a la que apunta el QR.</summary>
public record WebSettings(string Url, string JoinPath);
