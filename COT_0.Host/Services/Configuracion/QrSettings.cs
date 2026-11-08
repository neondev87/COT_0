// Sección "Qr" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>Tamaño de cada módulo y colores del QR ("#RRGGBB" o "#AARRGGBB").</summary>
public record QrSettings(int PixelsPerModule, string DarkColor, string LightColor);
