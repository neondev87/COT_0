// Sección "Tunnel" de appsettings.json (la lee también CloudflareTunnelService)
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>Ruta de cloudflared y cuántos segundos esperar el enlace público.</summary>
public record TunnelSettings(string CloudflaredPath, int StartTimeoutSeconds);
