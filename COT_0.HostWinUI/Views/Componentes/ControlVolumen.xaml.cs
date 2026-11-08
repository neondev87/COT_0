// Control de volumen compartido: recibe el ViewModel de música de quien lo usa
using COT_0.HostWinUI.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Componentes;

public sealed partial class ControlVolumen : UserControl
{
    // Propiedad de dependencia: así x:Bind se entera cuando la página asigna el ViewModel
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(MusicControlsViewModel), typeof(ControlVolumen), new PropertyMetadata(null));

    public ControlVolumen() => InitializeComponent();

    public MusicControlsViewModel ViewModel
    {
        get => (MusicControlsViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}
