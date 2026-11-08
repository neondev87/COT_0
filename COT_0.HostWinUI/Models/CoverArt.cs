// Portada grande lista para mostrar: imagen decodificada y su tamaño en pantalla (sin estirar ni recortar)
using Microsoft.UI.Xaml.Media.Imaging;

namespace COT_0.HostWinUI.Models;

/// <summary>Width/Height en píxeles lógicos: el recuadro toma este tamaño, así no sobra marco ni se recorta.</summary>
public record CoverArt(BitmapImage Image, double Width, double Height);
