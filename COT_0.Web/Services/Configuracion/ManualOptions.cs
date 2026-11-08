
namespace COT_0.Web.Services.Configuracion;

// Manual del invitado (sección "GuestManual" de appsettings.json); los valores son defaults
public class ManualOptions
{
    public const string Section = "GuestManual";

    // Audios del manual: carpeta local fuera del repo (relativa a Paths:MediaRoot), servida en AssetsPath
    public string AssetsFolder { get; set; } = "manual";
    public string AssetsPath { get; set; } = "/manual-assets";
    public string NoticeSound { get; set; } = "notificacion.mp3";

    // Lain en la tele: MP4 ligero sacado de gifs\lain.gif (muted, en bucle) y su primer cuadro de poster
    public string LainVideo { get; set; } = "lain.mp4";
    public string LainPoster { get; set; } = "lain-poster.jpg";

    // Arranque de la transmisión antes de la bienvenida
    public double BootSeconds { get; set; } = 2.4;

    // Avisos: el primero entra a los NoticeDelay s, uno cada NoticeInterval s; cada uno sale NoticeGap s antes del siguiente
    public double NoticeDelaySeconds { get; set; } = .6;
    public double NoticeIntervalSeconds { get; set; } = 4.7;
    public double NoticeGapSeconds { get; set; } = .55;

    public string NoticeAuthor { get; set; } = "Lain";
    public string WelcomeNotice { get; set; } = "Hola, soy Lain. Te acompaño en este recorrido.";
    public string FinishNotice { get; set; } = "Listo. Nos vemos en la Wired.";
    public string WelcomeHeading { get; set; } = "Bienvenido a COT_0";
    public string FinishHeading { get; set; } = "Ya sabes lo básico.";
    public string FinishText { get; set; } = "Entra cuando quieras.";
    public string FinishSignature { get; set; } = "Gracias por estar aquí. Vuelve pronto.";

    // Datos de muestra de las vistas (no se conectan a ninguna sala)
    public string DemoRoomCode { get; set; } = "DEMO";
    public string DemoImage { get; set; } = "img/bg-ascii.png";
    public int DemoImageThumbHeight { get; set; } = 135;
    public int DemoVoiceSeconds { get; set; } = 7;

    // Default en código: un array en appsettings se sumaría a este en vez de reemplazarlo
    public List<ManualStep> Steps { get; set; } = ManualSteps.Default();

    public string SoundUrl => AssetUrl(NoticeSound);
    public string VideoUrl => AssetUrl(LainVideo);
    public string PosterUrl => AssetUrl(LainPoster);

    private string AssetUrl(string file) => $"{AssetsPath.TrimEnd('/')}/{file}";
}
