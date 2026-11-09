// Atajos de teclado sin letrero: WinUI muestra un tooltip con la tecla ("Enter", "Esc", flechas) al pasar el mouse
// por cualquier elemento que tenga KeyboardAccelerators, salvo que su KeyboardAcceleratorPlacementMode sea Hidden
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace COT_0.Host.Services.Ventana;

/// <summary>
/// Navigator lo aplica a toda página al navegar; los botones lo traen de su estilo (implícito, TextButton, SolidButton,
/// BotonManualStyle). Un atajo nuevo en otro tipo de elemento: KeyboardAcceleratorPlacementMode="Hidden" en el XAML o Hide aquí.
/// </summary>
public static class ShortcutHints
{
    /// <summary>Quita el letrero sin tocar el atajo (sigue funcionando igual).</summary>
    public static void Hide(UIElement element) => element.KeyboardAcceleratorPlacementMode = KeyboardAcceleratorPlacementMode.Hidden;
}
