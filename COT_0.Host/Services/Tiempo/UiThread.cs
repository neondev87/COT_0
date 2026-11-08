// Pasa a la UI los eventos que llegan desde otros hilos (hub de SignalR, 7TV) sin bloquear a quien avisa
using Microsoft.UI.Dispatching;

namespace COT_0.Host.Services.Tiempo;

/// <summary>Si ya estás en el hilo de la UI, la acción corre en el momento.</summary>
public static class UiThread
{
    private static DispatcherQueue? _queue;

    /// <summary>Lo llama App al arrancar, desde el hilo de la UI.</summary>
    public static void Attach(DispatcherQueue queue) => _queue = queue;

    public static void Run(Action action)
    {
        if (_queue is null) return;
        if (_queue.HasThreadAccess) action();
        else _queue.TryEnqueue(() => action());
    }
}
