using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

/// <summary>Registrar usuario: la página le pasa su RegisterViewModel; la lógica vive ahí</summary>
public sealed partial class FormularioRegistro : UserControl
{
    // Propiedad de dependencia: así x:Bind se entera cuando la página asigna el ViewModel
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(RegisterViewModel), typeof(FormularioRegistro), new PropertyMetadata(null));

    public FormularioRegistro() => InitializeComponent();

    public RegisterViewModel ViewModel
    {
        get => (RegisterViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}
