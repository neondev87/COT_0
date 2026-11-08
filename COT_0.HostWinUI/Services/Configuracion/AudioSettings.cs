// Sección "Audio" de appsettings.json (misma que el WPF)
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>
/// Carpetas de música y portadas (relativas a Paths:MediaRoot), extensiones, volumen inicial y formatos.
/// CoverLargeSize: lado máximo de la portada grande; CoverMaxUpscale: cuánto se puede ampliar una portada chica.
/// </summary>
public record AudioSettings(
    string MusicFolder,
    IReadOnlyList<string> Extensions,
    string DefaultFile,
    double DefaultVolume,
    string CoversFolder,
    IReadOnlyList<string> CoverExtensions,
    int CoverDecodeSize,
    double CoverLargeSize,
    double CoverMaxUpscale,
    string DurationFormat);
