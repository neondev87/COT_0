// Sección "Audio" de appsettings.json (misma que el WPF)
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>Carpetas de música y portadas (relativas a Paths:MediaRoot), extensiones, volumen inicial y formatos.</summary>
public record AudioSettings(
    string MusicFolder,
    IReadOnlyList<string> Extensions,
    string DefaultFile,
    double DefaultVolume,
    string CoversFolder,
    IReadOnlyList<string> CoverExtensions,
    int CoverDecodeSize,
    string DurationFormat);
