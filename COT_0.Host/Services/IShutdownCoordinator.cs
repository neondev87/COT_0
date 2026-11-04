using System.ComponentModel;
using System.Windows;

namespace COT_0.Host.Services;

// Único camino de salida de la app y de la sala (✕, Alt+F4, barra de tareas, SALIR)
public interface IShutdownCoordinator
{
    // Handler síncrono de Window.Closing: cancela, confirma aparte y cierra cuando todo quedó liberado
    void OnWindowClosing(Window window, CancelEventArgs e);

    // Cerrar sesión: true si se puede seguir (no había sala, o se confirmó y se cerró)
    Task<bool> LeaveRoomAsync();
}
