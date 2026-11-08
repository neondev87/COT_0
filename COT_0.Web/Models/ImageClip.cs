namespace COT_0.Web.Models;

/// <summary>Imagen guardada en memoria</summary>
public sealed record ImageClip(string Id, byte[] Bytes, string ContentType, int Width, int Height);
