using COT_0.Core.Services;

namespace COT_0.Host.Services;

// Quick tunnel de cloudflared hacia localhost:5050
public class CloudflareTunnelService : ITunnelService
{
    public string? PublicUrl { get; private set; }
    public bool IsRunning { get; private set; }

    public Task<string> StartAsync(string localUrl, CancellationToken cancellationToken = default)
    {
        // TODO (Agente A): lanzar "cloudflared tunnel --url {localUrl}" y leer la URL pública
        throw new NotImplementedException();
    }

    public Task StopAsync()
    {
        // TODO (Agente A): cerrar el proceso de cloudflared
        throw new NotImplementedException();
    }
}
