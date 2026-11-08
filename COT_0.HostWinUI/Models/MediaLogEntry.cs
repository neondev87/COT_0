// Una línea del registro de medios (registro-medios.jsonl)
namespace COT_0.HostWinUI.Models;

/// <summary>
/// Qué falló al abrir un video, una imagen o al sacar un cuadro. ErrorCode va en hexadecimal (HRESULT)
/// para buscarlo tal cual; Detail es el motivo que dio Windows o ffmpeg.
/// </summary>
public record MediaLogEntry(DateTime Time, string Source, string File, string? ErrorCode, string? Detail);
