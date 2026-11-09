// Al cerrar la ventana (✕ propia de MainWindow o Alt+F4): siempre pregunta primero; luego cierra sala, túnel y servidor, y sí cierra
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

    // Cancel se pone antes de preguntar; si confirma, la ventana se cierra desde RequestCloseAsync
    private void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (_canClose) return;

        args.Cancel = true;
        _ = RequestCloseAsync();
    }

    /// <summary>Lo que hace la ✕: pregunta, apaga sala, túnel y servidor, y cierra.</summary>
    public async Task RequestCloseAsync()
    {
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
