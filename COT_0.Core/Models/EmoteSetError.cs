namespace COT_0.Core.Models;

/// <summary>Motivo concreto cuando el set no se pudo cargar (el Host lo muestra tal cual)</summary>
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
