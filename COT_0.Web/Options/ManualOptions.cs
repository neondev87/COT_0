using COT_0.Web.Models;

namespace COT_0.Web.Options;

// Manual del invitado (sección "GuestManual" de appsettings.json); los valores son defaults
public class ManualOptions
{
    public const string Section = "GuestManual";

    // Audios del manual: carpeta local fuera del repo (relativa a Paths:MediaRoot), servida en AssetsPath
    public string AssetsFolder { get; set; } = "manual";
    public string AssetsPath { get; set; } = "/manual-assets";
    public string NoticeSound { get; set; } = "notificacion.mp3";

    // Avisos: el primero entra a los NoticeDelay s, uno cada NoticeInterval s; cada uno sale NoticeGap s antes del siguiente
    public double NoticeDelaySeconds { get; set; } = .6;
    public double NoticeIntervalSeconds { get; set; } = 4.7;
    public double NoticeGapSeconds { get; set; } = .55;

    public string NoticeAuthor { get; set; } = "Lain";
    public string WelcomeNotice { get; set; } = "Hola, soy Lain. Te acompaño en este recorrido.";
    public string FinishNotice { get; set; } = "Listo. Nos vemos en la Wired.";

    // Datos de muestra de las vistas (no se conectan a ninguna sala)
    public string DemoRoomCode { get; set; } = "DEMO";
    public string DemoImage { get; set; } = "img/bg-ascii.png";
    public int DemoImageThumbHeight { get; set; } = 135;
    public int DemoVoiceSeconds { get; set; } = 7;

    // Default en código: un array en appsettings se sumaría a este en vez de reemplazarlo
    public List<ManualStep> Steps { get; set; } = ManualSteps.Default();

    public string SoundUrl => $"{AssetsPath.TrimEnd('/')}/{NoticeSound}";
}
