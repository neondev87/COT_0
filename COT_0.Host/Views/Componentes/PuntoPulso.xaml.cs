// Punto con pulso: arranca su animación al aparecer y la para al salir de la pantalla
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

public sealed partial class PuntoPulso : UserControl
{
    public PuntoPulso()
    {
        InitializeComponent();
        Loaded += (_, _) => PulseStoryboard.Begin();
        Unloaded += (_, _) => PulseStoryboard.Stop();
    }
}
