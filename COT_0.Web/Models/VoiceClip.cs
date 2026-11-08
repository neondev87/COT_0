namespace COT_0.Web.Models;

/// <summary>Nota de voz guardada en memoria</summary>
public sealed record VoiceClip(string Id, byte[] Bytes, string ContentType, TimeSpan Duration);
