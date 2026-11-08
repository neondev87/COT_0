namespace COT_0.Web.Services.Configuracion;

// Notas de voz (sección "Voice" de appsettings.json); los valores son defaults
public class VoiceOptions
{
    public const string Section = "Voice";

    // Ruta donde se sirven los audios: {Path}/{id}
    public string Path { get; set; } = "/voice";

    // Formatos en orden de preferencia: MP4/AAC suena en iPhone; el WebM se pasa a WAV en el servidor.
    // Default en código: un array en appsettings se sumaría a este en vez de reemplazarlo
    public string[] MimeCandidates { get; set; } = ["audio/mp4;codecs=mp4a.40.2", "audio/webm;codecs=opus", "audio/webm", "audio/mp4"];

    public int MaxSeconds { get; set; } = 60;
    public int MinMilliseconds { get; set; } = 500;
    public long MaxBytes { get; set; } = 4 * 1024 * 1024;

    // Tope de audios en memoria (se van los más viejos); se borran todos al cerrar la sala
    public int MaxClips { get; set; } = 200;

    // Frecuencia del WAV al convertir los WebM/Opus
    public int WavSampleRate { get; set; } = 24000;
}
