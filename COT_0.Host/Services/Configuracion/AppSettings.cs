// Secciones de appsettings.json que usa el Host
namespace COT_0.Host.Services.Configuracion;

/// <summary>Raíz de la configuración (las demás secciones las lee la web).</summary>
public record AppSettings(
    WindowSettings Window,
    StorageSettings Storage,
    SecuritySettings Security,
    InputsSettings Inputs,
    UsuariosSettings Usuarios,
    AdminSettings Admin,
    StoreSettings Store,
    RecoverySettings Recovery,
    UiSettings Ui,
    PathsSettings Paths,
    AudioSettings Audio,
    BackgroundSettings Background,
    ManualSettings Manual,
    BrandingSettings Branding,
    TunnelSettings Tunnel,
    QrSettings Qr,
    WebSettings Web);
