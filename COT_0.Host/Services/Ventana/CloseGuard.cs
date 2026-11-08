// Al cerrar la ventana: con sala activa pregunta primero; luego cierra sala, túnel y servidor, y sí cierra
using COT_0.Host.Services.Sala;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace COT_0.Host.Services.Ventana;

/// <summary>Mismo "¿Cerrar la sala y salir?" que SALIR del dashboard (RoomSession.ConfirmLeaveAsync).</summary>
public class CloseGuard(RoomSession session)
{
    private Window? _window;
    private bool _canClose;
    private bool _isClosing;

    public void Attach(Window window)
    {
        _window = window;
        window.AppWindow.Closing += OnClosing;
    }

    // async void: manejador de evento. Cancel se pone antes del primer await
    private async void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (_canClose) return;

        args.Cancel = true;
        if (_isClosing) return;
        if (session.IsActive && !await session.ConfirmLeaveAsync()) return;

        _isClosing = true;
        try
        {
            await session.ShutdownAsync();
        }
        finally
        {
            // Aunque algo falle al apagar, la ventana se cierra (cloudflared muere con KillOnCloseJob)
            _canClose = true;
            _window?.Close();
        }
    }
}
