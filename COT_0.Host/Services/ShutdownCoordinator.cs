using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using COT_0.Host.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace COT_0.Host.Services;

// Cierre ordenado: sala → túnel → música → fondos → servidor web → ventana.
// Si un paso falla se registra, se avisa y se sale igual
public sealed class ShutdownCoordinator(
    RoomSession session,
    WebServerHost webServer,
    MusicService music,
    BackgroundPickerViewModel backgrounds,
    DialogService dialogs,
    IConfiguration configuration,
    ILogger<ShutdownCoordinator> logger) : IShutdownCoordinator
{
    // Hay un diálogo o una liberación en curso: más ✕ / Alt+F4 / SALIR se ignoran
    private bool _busy;

    // Todo liberado: la próxima pasada por Closing deja cerrar sin preguntar
    private bool _exitConfirmed;

    public void OnWindowClosing(Window window, CancelEventArgs e)
    {
        if (_exitConfirmed) return;

        e.Cancel = true;
        if (_busy) return;
        _busy = true;
        _ = ExitAsync(window);
    }

    public async Task<bool> LeaveRoomAsync()
    {
        if (_busy) return false;
        _busy = true;
        try
        {
            if (session.IsActive && !session.ConfirmLeave()) return false;
            await RunStepsAsync((Text("Room"), session.CloseRoomAsync), (Text("Tunnel"), session.StopTunnelAsync));
            return true;
        }
        finally
        {
            _busy = false;
        }
    }

    // Nunca lanza: cualquier error se registra y la ventana se cierra igual
    private async Task ExitAsync(Window window)
    {
        try
        {
            // Turno nuevo del dispatcher: el diálogo no se abre dentro de Closing
            await Dispatcher.Yield(DispatcherPriority.Background);
            if (session.IsActive && !session.ConfirmLeave()) return;

            await RunStepsAsync(
                (Text("Room"), session.CloseRoomAsync),
                (Text("Tunnel"), session.StopTunnelAsync),
                (Text("Media"), ReleaseMedia),
                (Text("WebServer"), webServer.StopAsync));
            CloseLater(window);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado al salir");
            CloseLater(window);
        }
        finally
        {
            _busy = false;
        }
    }

    private Task ReleaseMedia()
    {
        music.Stop();
        backgrounds.Release();
        return Task.CompletedTask;
    }

    // WPF todavía considera la ventana "en cierre" hasta que termina Closing: se cierra en otro turno
    private void CloseLater(Window window)
    {
        _exitConfirmed = true;
        window.Dispatcher.BeginInvoke(window.Close);
    }

    private async Task RunStepsAsync(params (string Name, Func<Task> Run)[] steps)
    {
        var failures = new List<string>();
        foreach (var (name, run) in steps)
        {
            try
            {
                await run();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falló el cierre de {Step}", name);
                failures.Add($"{name}: {ex.Message}");
            }
        }
        if (failures.Count > 0)
            dialogs.ShowError(Text("ErrorTitle"), string.Join("\n", failures));
    }

    private string Text(string key) => configuration.GetText("Shutdown:" + key);
}
