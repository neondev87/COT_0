// Sección "Manual" de appsettings.json (rutas relativas a Paths:MediaRoot, fuera del repo)
namespace COT_0.HostWinUI.Services.Configuracion;

/// <summary>
/// Videos y sonidos de Lain, carpeta opcional con una captura por paso (paso1.png…) y a qué ancho se decodifica, volúmenes,
/// segundos en que entra cada aviso, duración del cambio de paso y cuántos avisos tiene cada paso.
/// </summary>
public record ManualSettings(
    string LainVideo,
    string FarewellVideo,
    string NotificationSound,
    string StaticSound,
    string StagesFolder,
    string StageExtension,
    uint StageDecodeSize,
    double NotificationVolume,
    double StaticVolume,
    IReadOnlyList<double> CueSeconds,
    int StepChangeMilliseconds,
    IReadOnlyList<int> NoticeCounts);
