// Sección "Background" de appsettings.json (carpeta, extensiones, velo y topes del cuadro fijo)
namespace COT_0.Host.Services.Configuracion;

/// <summary>
/// Carpeta de fondos (relativa a Paths:MediaRoot), extensiones, fondo inicial, desde cuántos MB un video
/// arranca como cuadro fijo, tamaños de decodificación y opacidad del velo (normal y con el selector abierto).
/// FrameSeconds/FrameMaxWidth: el cuadro que se saca de cada video (miniatura y cuadro fijo) y CacheFolder,
/// su carpeta dentro de Storage:CacheFolder. FfmpegPath: "ffmpeg" lo busca junto a la app y en el PATH.
/// VideoMuted: sonido normal del video de fondo fuera del manual (true = en silencio); en el manual siempre va mudo.
/// </summary>
public record BackgroundSettings(
    string Folder,
    IReadOnlyList<string> ImageExtensions,
    IReadOnlyList<string> VideoExtensions,
    string DefaultFile,
    double HeavyVideoMegabytes,
    uint ThumbnailSize,
    uint StillSize,
    double VeilOpacity,
    double PickerVeilOpacity,
    bool VideoMuted,
    string CacheFolder,
    double FrameSeconds,
    uint FrameMaxWidth,
    string FfmpegPath,
    int FfmpegTimeoutSeconds);
