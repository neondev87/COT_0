// Miniatura de un emote con su nombre (stock de un usuario y tickets)
using COT_0.Host.Services.Configuracion;
using Microsoft.UI.Xaml.Media.Imaging;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Se crea en el hilo de la UI (BitmapImage lo exige). Baja del CDN de 7TV al pintarse (las listas virtualizan)
/// y sin animar (AutoPlay = false): con muchas miniaturas la app sigue ligera.
/// </summary>
public sealed class EmoteThumbViewModel(string name, string url)
{
    public string Name { get; } = name;

    public BitmapImage Thumbnail { get; } = Create(url);

    public static BitmapImage Create(string url) => new(new Uri(url))
    {
        DecodePixelWidth = AppConfig.Settings.Store.ThumbnailSize,
        AutoPlay = false,
    };
}
