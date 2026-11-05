using COT_0.Core.Models;

namespace COT_0.Host.ViewModels;

// Fila de emote en las listas de reportes y de quitados
public sealed class EmoteRowViewModel(RoomEmote emote, string detail)
{
    public string Id { get; } = emote.Id;
    public string Name { get; } = emote.Name;
    public string Detail { get; } = detail;

    // WPF no decodifica webp: se usa la alternativa (gif, primer cuadro) si existe
    public string Thumbnail { get; } = emote.Small.FallbackUrl ?? emote.Small.StillUrl ?? emote.Small.Url;
}
