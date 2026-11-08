namespace COT_0.Host.Models;

/// <summary>Por qué no se pudo abrir la sala (cada valor tiene su Text.Room.Error.*)</summary>
public enum RoomError
{
    CloudflaredMissing,
    TunnelTimeout,
    TunnelFailed,
    PortInUse,
    ServerFailed,
}
