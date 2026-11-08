// Secciones de appsettings.json que usa el Host WinUI
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>Raíz de la configuración (las demás secciones las lee la web).</summary>
public record AppSettings(
    WindowSettings Window,
    StorageSettings Storage,
    SecuritySettings Security,
    ValidationSettings Validation,
    RecoverySettings Recovery,
    UiSettings Ui,
    PathsSettings Paths,
    AudioSettings Audio,
    LinksSettings Links);
