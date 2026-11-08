namespace COT_0.Core.Services;

/// <summary>Túnel público hacia el servidor local (cloudflared quick tunnel)</summary>
public interface ITunnelService
{
    // URL pública (https://xxxx.trycloudflare.com), null si no está activo
    string? PublicUrl { get; }
    bool IsRunning { get; }

    // Levanta el túnel hacia localUrl y devuelve la URL pública
    Task<string> StartAsync(string localUrl, CancellationToken cancellationToken = default);
    Task StopAsync();
}
