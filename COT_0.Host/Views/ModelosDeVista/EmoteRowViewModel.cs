// Fila de emote en las listas de reportes y de quitados
using COT_0.Core.Models;
using Microsoft.UI.Xaml.Media.Imaging;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Se crea en el hilo de la UI (BitmapImage lo exige); la miniatura baja del CDN de 7TV, nada en disco.</summary>
public sealed class EmoteRowViewModel(RoomEmote emote, string detail, int thumbnailSize)
{
    public string Id { get; } = emote.Id;
    public string Name { get; } = emote.Name;
    public string Detail { get; } = detail;

    // Se prefiere el gif (WinUI lo anima sin códecs extra); webp solo si no hay otro
    public BitmapImage Thumbnail { get; } = new(new Uri(emote.Small.FallbackUrl ?? emote.Small.Url))
    {
        DecodePixelWidth = thumbnailSize,
    };
}
