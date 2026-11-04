using System.Windows.Threading;

namespace COT_0.Host.Services;

// Reloj compartido: hora local y un tick alineado al cambio de cada segundo
public class ClockService
{
    private const int AlignMarginMilliseconds = 5;

    private readonly DispatcherTimer _timer = new(DispatcherPriority.Render);

    public ClockService()
    {
        _timer.Tick += (_, _) => OnTick();
        ScheduleNext();
        _timer.Start();
    }

    public event Action? Tick;

    public DateTime Now => DateTime.Now;

    // Las fechas de Core vienen en UTC: se pasan a hora local antes de mostrar o restar
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
    private void ScheduleNext() =>
        _timer.Interval = TimeSpan.FromMilliseconds(1000 - Now.Millisecond + AlignMarginMilliseconds);
}
