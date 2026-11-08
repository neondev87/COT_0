// Sección "Qr" de appsettings.json (misma que el WPF)
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>Tamaño de cada módulo y colores del QR ("#RRGGBB" o "#AARRGGBB").</summary>
public record QrSettings(int PixelsPerModule, string DarkColor, string LightColor);
