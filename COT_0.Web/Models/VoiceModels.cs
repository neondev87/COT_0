namespace COT_0.Web.Models;

// Formato real del audio subido, según sus magic bytes
public enum AudioFormat
{
    Unknown,
    Mp4Aac,
    WebmOpus,
}

// Estado del micrófono del invitado
public enum VoiceState
{
    Idle,
    Recording,
    Sending,
}

// Nota de voz guardada en memoria
public sealed record VoiceClip(string Id, byte[] Bytes, string ContentType, TimeSpan Duration);

// Lo que necesita la burbuja para reproducir; Available = false si ya no está en memoria
public sealed record VoicePlayback(string Url, TimeSpan Duration, bool Available);
