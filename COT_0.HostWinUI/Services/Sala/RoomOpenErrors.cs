// Traduce el error al abrir la sala (servidor o túnel) a un RoomError con su texto
using System.ComponentModel;
using System.Net.Sockets;
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Textos;
using Microsoft.AspNetCore.Connections;

namespace COT_0.HostWinUI.Services.Sala;

/// <summary>Los textos usan {0} ruta de cloudflared, {1} URL local, {2} segundos de espera y {3} detalle.</summary>
public static class RoomOpenErrors
{
    /// <summary>Errores que puede soltar RoomSession.OpenAsync (Kestrel o cloudflared).</summary>
    public static bool IsOpenError(Exception ex) =>
        ex is IOException or InvalidOperationException or SocketException or TimeoutException or Win32Exception;

    /// <summary>serverRunning distingue un InvalidOperationException del túnel de uno del servidor.</summary>
    public static RoomError Classify(Exception ex, bool serverRunning) => ex switch
    {
        FileNotFoundException => RoomError.CloudflaredMissing,
        TimeoutException => RoomError.TunnelTimeout,
        _ when IsPortInUse(ex) => RoomError.PortInUse,
        _ when serverRunning => RoomError.TunnelFailed,
        _ => RoomError.ServerFailed,
    };

    public static string Message(RoomError error, Exception ex) => string.Format(
        UiText.Get($"Text.Room.Error.{error}"),
        AppConfig.Settings.Tunnel.CloudflaredPath,
        AppConfig.Settings.Web.Url,
        AppConfig.Settings.Tunnel.StartTimeoutSeconds,
        ex.Message);

    // Kestrel envuelve el AddressInUseException (o el SocketException) en un IOException
    private static bool IsPortInUse(Exception ex)
    {
        for (var current = ex; current is not null; current = current.InnerException)
        {
            if (current is AddressInUseException) return true;
            if (current is SocketException { SocketErrorCode: SocketError.AddressAlreadyInUse }) return true;
        }
        return false;
    }
}
