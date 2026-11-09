// Sección "Audio" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>
/// Carpetas de música y portadas (relativas a Paths:MediaRoot), extensiones, volumen inicial (DefaultVolume, 0 a 100) y formatos.
/// InitialTrack: número de la pista que suena al abrir; TrackNameFormat lo vuelve nombre de archivo sin extensión (2 → "Audio_02").
/// CoverLargeSize: lado máximo de la portada grande; CoverMaxUpscale: cuánto se puede ampliar una portada chica.
/// </summary>
public record AudioSettings(
    string MusicFolder,
    IReadOnlyList<string> Extensions,
    int InitialTrack,
    string TrackNameFormat,
    double DefaultVolume,
    string CoversFolder,
    IReadOnlyList<string> CoverExtensions,
    int CoverDecodeSize,
    double CoverLargeSize,
    double CoverMaxUpscale,
    string DurationFormat);
