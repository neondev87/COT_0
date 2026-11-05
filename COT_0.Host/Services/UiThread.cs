using System.Windows;

namespace COT_0.Host.Services;

// Los eventos de la web (hub, 7TV) llegan desde otros hilos: se pasan a la UI sin bloquear al que avisa.
// Si ya se está en la UI (ej. el Host abrió la sala) se ejecuta en el momento
public static class UiThread
{
    public static void Run(Action action)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null) return;
        if (dispatcher.CheckAccess()) action();
        else dispatcher.BeginInvoke(action);
    }
}
