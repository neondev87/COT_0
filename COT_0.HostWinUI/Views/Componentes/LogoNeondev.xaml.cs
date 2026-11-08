// Logo Neondev: toma su ViewModel del contenedor y pone la mano como cursor (ProtectedCursor solo se puede desde la clase)
using COT_0.HostWinUI.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Componentes;

public sealed partial class LogoNeondev : UserControl
{
    public LogoNeondevViewModel ViewModel { get; } = App.Services.GetRequiredService<LogoNeondevViewModel>();

    public LogoNeondev()
    {
        InitializeComponent();
        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }
}
