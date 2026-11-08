// Recuperar contraseña: la página le pasa su RecoveryViewModel; la lógica vive ahí
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

public sealed partial class FormularioRecuperacion : UserControl
{
    // Propiedad de dependencia: así x:Bind se entera cuando la página asigna el ViewModel
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(RecoveryViewModel), typeof(FormularioRecuperacion), new PropertyMetadata(null));

    public FormularioRecuperacion() => InitializeComponent();

    public RecoveryViewModel ViewModel
    {
        get => (RecoveryViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}
