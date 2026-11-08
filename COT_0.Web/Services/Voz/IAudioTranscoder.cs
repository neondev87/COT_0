namespace COT_0.Web.Services.Voz;

/// <summary>Pasa un WebM/Opus a WAV para que también suene en Safari/iPhone; null si no se pudo leer</summary>
public interface IAudioTranscoder
{
    VoiceClip? WebmToWav(string id, byte[] webm);
}
