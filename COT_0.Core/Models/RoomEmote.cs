namespace COT_0.Core.Models;

/// <summary>Emote de un set de 7TV. Las imágenes se cargan directo del CDN (no se guardan en disco)</summary>
public sealed record RoomEmote(string Id, string Name, int Width, int Height, bool Animated, EmoteSources Small, EmoteSources Large);
