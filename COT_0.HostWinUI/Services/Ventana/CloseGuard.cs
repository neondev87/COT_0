// Al cerrar la ventana con sala activa: pregunta, cierra la sala (y su sesión) y el túnel, y luego sí cierra
using COT_0.HostWinUI.Services.Sala;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace COT_0.HostWinUI.Services.Ventana;

/// <summary>Mismo "¿Cerrar la sala y salir?" que SALIR del dashboard (RoomSession.ConfirmLeaveAsync).</summary>
public class CloseGuard(RoomSession session)
{
    private Window? _window;
    private bool _canClose;

    public void Attach(Window window)
    {
        _window = window;
        window.AppWindow.Closing += OnClosing;
    }

    // async void: manejador de evento. Cancel se pone antes del primer await
    private async void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (_canClose || !session.IsActive) return;

        args.Cancel = true;
        if (!await session.ConfirmLeaveAsync()) return;

        await session.CloseAsync();
        _canClose = true;
        _window?.Close();
    }
}
