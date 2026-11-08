// Color del estado del set de 7TV: OK claro, error en acento, el resto apagado
using COT_0.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace COT_0.Host.Views.Componentes;

/// <summary>Lo usa PanelEmotes con x:Bind.</summary>
public static class EmoteStatusBrushes
{
    public static Brush For(EmoteSetStatus status) => (Brush)Application.Current.Resources[status switch
    {
        EmoteSetStatus.Ready => "LightBrush",
        EmoteSetStatus.NotFound or EmoteSetStatus.Failed => "AccentBrush",
        _ => "Light55Brush",
    }];
}
