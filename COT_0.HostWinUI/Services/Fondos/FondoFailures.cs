// Traduce el error de Windows al abrir un fondo (HRESULT de MediaFailed o de una excepción) a un FondoError
using System.Runtime.InteropServices;
using COT_0.HostWinUI.Models;
using Windows.Media.Playback;

namespace COT_0.HostWinUI.Services.Fondos;

/// <summary>Así el InfoBar de FONDOS dice la causa en español sencillo y no solo "no se pudo abrir".</summary>
public static class FondoFailures
{
    private const int FileNotFound = unchecked((int)0x80070002);
    private const int PathNotFound = unchecked((int)0x80070003);
    private const int AccessDenied = unchecked((int)0x80070005);
    private const int SharingViolation = unchecked((int)0x80070020);
    private const int LockViolation = unchecked((int)0x80070021);
    private const int CodecNotFound = unchecked((int)0xC00D5212);         // MF_E_TOPO_CODEC_NOT_FOUND
    private const int UnsupportedByteStream = unchecked((int)0xC00D36C4); // MF_E_UNSUPPORTED_BYTESTREAM_TYPE
    private const int InvalidMediaType = unchecked((int)0xC00D36B4);      // MF_E_INVALIDMEDIATYPE
    private const int UnsupportedFormat = unchecked((int)0xC00D36B3);     // MF_E_UNSUPPORTED_FORMAT

    public static FondoError FromMedia(MediaPlayerError error, int hresult) => hresult switch
    {
        FileNotFound or PathNotFound => FondoError.FileMissing,
        AccessDenied or SharingViolation or LockViolation => FondoError.FileLocked,
        CodecNotFound or UnsupportedByteStream or InvalidMediaType or UnsupportedFormat => FondoError.VideoCodec,
        _ => error == MediaPlayerError.SourceNotSupported ? FondoError.VideoCodec : FondoError.VideoDecode,
    };

    public static FondoError FromException(Exception ex) => FromMedia(MediaPlayerError.Unknown, ex.HResult);

    /// <summary>Errores esperables al leer un archivo de medios con las APIs de Windows.</summary>
    public static bool IsMediaException(Exception ex) =>
        ex is IOException or UnauthorizedAccessException or ArgumentException or COMException or InvalidOperationException;
}
