// Convierte el número de tono de una fila en el pincel AvatarTone{n}Brush de App.xaml
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace COT_0.Host.Views.Convertidores;

/// <summary>Lo usa la lista de conectados con x:Bind (este XAML no tiene AlternationIndex).</summary>
public static class AvatarTones
{
    public static Brush For(int tone) => (Brush)Application.Current.Resources[$"AvatarTone{tone}Brush"];
}
