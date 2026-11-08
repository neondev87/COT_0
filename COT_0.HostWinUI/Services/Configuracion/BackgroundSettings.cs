// Sección "Background" de appsettings.json (la del WPF, sin PreviewSeconds y con los topes de WinUI)
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>
/// Carpeta de fondos (relativa a Paths:MediaRoot), extensiones, fondo inicial, desde cuántos MB un video
/// arranca como cuadro fijo, tamaños de decodificación y opacidad del velo (normal y con el selector abierto).
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
    double PickerVeilOpacity);
