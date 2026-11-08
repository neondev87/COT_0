// Reloj compartido: hora local y un tick alineado al cambio de cada segundo
using Microsoft.UI.Dispatching;

namespace COT_0.Host.Services.Tiempo;

/// <summary>Con DispatcherQueueTimer: el Tick llega en el hilo de la UI.</summary>
public class ClockService
{
    private const int AlignMarginMilliseconds = 5;

    private readonly DispatcherQueueTimer _timer;

    // Se crea en el hilo de la UI (al armar los servicios en App)
    public ClockService()
    {
        _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _timer.IsRepeating = false;
        _timer.Tick += (_, _) => OnTick();
        ScheduleNext();
    }

    public event Action? Tick;

    public DateTime Now => DateTime.Now;

    // Las fechas de Core vienen en UTC: pásalas a hora local antes de mostrar o restar
    public static DateTime ToLocal(DateTime value) =>
        value.Kind == DateTimeKind.Local ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc).ToLocalTime();

    public TimeSpan Since(DateTime value)
    {
        var elapsed = Now - ToLocal(value);
        return elapsed < TimeSpan.Zero ? TimeSpan.Zero : elapsed;
    }

    private void OnTick()
    {
        ScheduleNext();
        Tick?.Invoke();
    }

    // Reprograma hasta el próximo segundo exacto para no acumular desfase
    private void ScheduleNext()
    {
        _timer.Interval = TimeSpan.FromMilliseconds(1000 - Now.Millisecond + AlignMarginMilliseconds);
        _timer.Start();
    }
}
