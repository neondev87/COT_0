// Sección "Manual" de appsettings.json (rutas relativas a Paths:MediaRoot, fuera del repo)
namespace COT_0.Host.Services.Configuracion;

/// <summary>
/// Videos y sonidos de Lain, carpeta opcional con una captura por paso (paso1.png…) y a qué ancho se decodifica, volúmenes,
/// segundos en que entra cada aviso, duración del cambio de paso, cuántos avisos tiene cada paso
/// y los datos ficticios de las vistas de ejemplo (Demo).
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
    IReadOnlyList<int> NoticeCounts,
    ManualDemoSettings Demo);
