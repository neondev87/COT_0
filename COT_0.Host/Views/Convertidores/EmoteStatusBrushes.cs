// Color del estado de los emotes de la sala: con comprados en claro, sin emotes apagado
using COT_0.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace COT_0.Host.Views.Convertidores;

/// <summary>Lo usan PanelEmotes y la vista de ejemplo del manual con x:Bind.</summary>
public static class EmoteStatusBrushes
{
    public static Brush For(EmoteSetStatus status) =>
        (Brush)Application.Current.Resources[status == EmoteSetStatus.Ready ? "LightBrush" : "Light55Brush"];
}
