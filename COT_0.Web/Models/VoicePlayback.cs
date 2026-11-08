namespace COT_0.Web.Models;

/// <summary>Lo que necesita la burbuja para reproducir; Available = false si ya no está en memoria</summary>
public sealed record VoicePlayback(string Url, TimeSpan Duration, bool Available);
