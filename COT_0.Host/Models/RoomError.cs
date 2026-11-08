// Por qué no se pudo abrir la sala (cada valor tiene su Text.Room.Error.*)
namespace COT_0.Host.Models;

public enum RoomError
{
    CloudflaredMissing,
    TunnelTimeout,
    TunnelFailed,
    PortInUse,
    ServerFailed,
}
