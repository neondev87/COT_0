// Color del estado del set de 7TV: OK claro, error en acento, el resto apagado
using COT_0.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace COT_0.HostWinUI.Views.Componentes;

/// <summary>Lo usa PanelEmotes con x:Bind (reemplaza los 3 DataTrigger del WPF).</summary>
public static class EmoteStatusBrushes
{
    public static Brush For(EmoteSetStatus status) => (Brush)Application.Current.Resources[status switch
    {
        EmoteSetStatus.Ready => "LightBrush",
        EmoteSetStatus.NotFound or EmoteSetStatus.Failed => "AccentBrush",
        _ => "Light55Brush",
    }];
}
