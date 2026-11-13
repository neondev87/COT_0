using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>Sello de compra: solo arranca su animación cada vez que se hace visible (lo decide PurchaseSummaryViewModel)</summary>
public sealed partial class SelloCompra : UserControl
{
    public SelloCompra()
    {
        InitializeComponent();
        RegisterPropertyChangedCallback(VisibilityProperty, (_, _) => Play());
    }

    private void Play()
    {
        if (Visibility != Visibility.Visible) return;
        var effect = (Storyboard)Resources["SealEffect"];
        effect.Stop();
        effect.Begin();
    }
}
