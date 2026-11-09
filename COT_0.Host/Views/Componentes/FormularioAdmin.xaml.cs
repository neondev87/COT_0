using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

/// <summary>Modo ADMIN del login: la página le pasa su AdminLoginViewModel; la lógica vive ahí</summary>
public sealed partial class FormularioAdmin : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(AdminLoginViewModel), typeof(FormularioAdmin), new PropertyMetadata(null));

    public FormularioAdmin() => InitializeComponent();

    public AdminLoginViewModel ViewModel
    {
        get => (AdminLoginViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
}
