// Prepara la ventana: siempre en pantalla completa (sin barra de título ni botones de Windows; la ✕ va en MainWindow.xaml)
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace COT_0.Host.Services.Ventana;

/// <summary>Pantalla completa al iniciar con AppWindowPresenterKind.FullScreen; guarda el HWND para los selectores de archivos.</summary>
public static class WindowSetup
{
    /// <summary>HWND de la ventana principal (FileOpenPicker lo necesita en una app sin empaquetar).</summary>
    public static nint Handle { get; private set; }

    public static void Apply(Window window)
    {
        Handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
        window.AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
    }
}
