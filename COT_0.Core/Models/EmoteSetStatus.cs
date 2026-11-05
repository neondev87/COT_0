namespace COT_0.Core.Models;

public enum EmoteSetStatus
{
    Off,
    Loading,
    Ready,
    NotFound,
    Failed,
}

// Motivo concreto cuando el set no se pudo cargar (el Host lo muestra tal cual)
public enum EmoteSetError
{
    None,
    InvalidId,
    NotFound,
    Timeout,
    Network,
    HttpError,
    BadJson,
}
