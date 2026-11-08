// Cambia de página dentro del Frame de la ventana principal
using COT_0.HostWinUI.Views.Pantallas;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Services.Ventana;

/// <summary>Un solo Frame para toda la app: cada pantalla es una Page.</summary>
public static class Navigator
{
    private static Frame? _frame;

    /// <summary>Lo llama MainWindow al arrancar con su RootFrame.</summary>
    public static void Attach(Frame frame) => _frame = frame;

    /// <summary>Raíz visual de la ventana: la necesitan los ContentDialog para saber dónde abrirse.</summary>
    public static XamlRoot? XamlRoot => _frame?.XamlRoot;

    public static void ShowLogin() => Go<LoginPage>();

    public static void ShowDashboard() => Go<DashboardPage>();

    public static void ShowRoom() => Go<SalaPage>();

    public static void ShowManual() => Go<ManualPage>();

    private static void Go<TPage>() where TPage : Page
    {
        if (_frame is null || _frame.Content is TPage) return;
        _frame.Navigate(typeof(TPage));
    }
}
