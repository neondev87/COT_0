using System.Windows.Input;
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes.Usuarios;

/// <summary>Cambiar nombre o contraseña: la página le pasa el AccountFormViewModel abierto y el comando para cerrar</summary>
public sealed partial class RecuadroCuenta : UserControl
{
    public static readonly DependencyProperty FormProperty = DependencyProperty.Register(
        nameof(Form), typeof(AccountFormViewModel), typeof(RecuadroCuenta), new PropertyMetadata(null));

    public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(
        nameof(CloseCommand), typeof(ICommand), typeof(RecuadroCuenta), new PropertyMetadata(null));

    public RecuadroCuenta() => InitializeComponent();

    public AccountFormViewModel? Form
    {
        get => (AccountFormViewModel?)GetValue(FormProperty);
        set => SetValue(FormProperty, value);
    }

    public ICommand? CloseCommand
    {
        get => (ICommand?)GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }
}
