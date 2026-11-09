// Al cerrar la ventana: siempre pregunta primero; luego cierra sala, túnel y servidor, y sí cierra
using COT_0.Host.Services.Sala;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace COT_0.Host.Services.Ventana;

/// <summary>Con sala activa, mismo "¿Cerrar la sala y salir?" que SALIR; sin sala, "¿Salir de COT_0?".</summary>
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
        // Un ✕ más mientras pregunta o apaga no vuelve a abrir el diálogo ni a cerrar la ventana
        if (_isClosing) return;
        _isClosing = true;
        if (!await session.ConfirmCloseAppAsync())
        {
            _isClosing = false;
            return;
        }

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
