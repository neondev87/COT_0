// Revisa si el puerto de Web:Url está libre antes de dárselo al servidor web
using System.Net;
using System.Net.Sockets;

namespace COT_0.Host.Services.Sala;

/// <summary>
/// El servidor web no se puede volver a arrancar después de fallar al enlazar el puerto;
/// por eso se prueba antes con un listener que se suelta enseguida.
/// </summary>
public static class PortProbe
{
    /// <summary>Lanza SocketException (AddressAlreadyInUse) si otra app ya usa el puerto.</summary>
    public static void ThrowIfInUse(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return;

        foreach (var address in AddressesOf(uri.Host))
        {
            var listener = new TcpListener(address, uri.Port);
            listener.Start();
            listener.Stop();
        }
    }

    /// <summary>Puerto de Web:Url (5050) para el mensaje de puerto ocupado.</summary>
    public static int PortOf(string url) => Uri.TryCreate(url, UriKind.Absolute, out var uri) ? uri.Port : 0;

    // "localhost" enlaza las dos direcciones de loopback, igual que el servidor web
    private static IEnumerable<IPAddress> AddressesOf(string host)
    {
        if (IPAddress.TryParse(host.Trim('[', ']'), out var address)) return [address];
        if (!host.Equals("localhost", StringComparison.OrdinalIgnoreCase)) return [];
        return Socket.OSSupportsIPv6 ? [IPAddress.Loopback, IPAddress.IPv6Loopback] : [IPAddress.Loopback];
    }
}
