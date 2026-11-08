// Prepara la ventana: tamaño fijo del diseño, centrada y con la barra de título integrada al fondo
using System.Runtime.InteropServices;
using COT_0.Host.Services.Configuracion;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using WinRT.Interop;

namespace COT_0.Host.Services.Ventana;

/// <summary>1280×720 lógicos, se puede minimizar pero no maximizar ni redimensionar.</summary>
public static class WindowSetup
{
    private const double BaseDpi = 96.0;

    public static void Apply(Window window)
    {
        // Sin SetTitleBar: Windows deja arrastrar la franja de arriba (32 px) y pinta sus botones encima
        window.ExtendsContentIntoTitleBar = true;

        var appWindow = window.AppWindow;
        appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
        }

        // AppWindow trabaja en pixeles físicos: escala el tamaño del diseño según el DPI del monitor
        var scale = GetDpiForWindow(WindowNative.GetWindowHandle(window)) / BaseDpi;
        var size = AppConfig.Settings.Window;
        var width = (int)(size.Width * scale);
        var height = (int)(size.Height * scale);

        var area = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Primary).WorkArea;
        appWindow.MoveAndResize(new RectInt32(
            area.X + (area.Width - width) / 2,
            area.Y + (area.Height - height) / 2,
            width,
            height));
    }

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hwnd);
}
