// Prepara la ventana: siempre en pantalla completa (sin barra de título ni botones de Windows; la ✕ va en MainWindow.xaml)
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace COT_0.Host.Services.Ventana;

/// <summary>Pantalla completa al iniciar con AppWindowPresenterKind.FullScreen.</summary>
public static class WindowSetup
{
    public static void Apply(Window window) => window.AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
}
