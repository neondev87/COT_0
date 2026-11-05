using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Tiempos de arranque por fase (Diagnostics:StartupTrace) en Diagnostics:StartupLogFile.
// Apagado no hace nada; mientras corre también anota cuándo el hilo de la UI queda trabado
public static class StartupTrace
{
    private static readonly object Gate = new();
    private static StreamWriter? _log;
    private static DateTime _processStart;
    private static TimeSpan _last;
    private static DispatcherTimer? _watchdog;
    private static long _lastBeat;
    private static int _stallMilliseconds;

    private static readonly List<(DateTime At, string Phase)> Early = [];

    // Antes de leer la configuración solo se guarda la hora
    public static void Remember(string phase) => Early.Add((DateTime.Now, phase));

    public static void Enable(IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("Diagnostics:StartupTrace")) return;

        var file = Path.Combine(AppContext.BaseDirectory, configuration["Diagnostics:StartupLogFile"] ?? "startup.log");
        try
        {
            _log = new StreamWriter(file, append: false) { AutoFlush = true };
        }
        catch (IOException)
        {
            // Otro proceso tiene el log abierto: se arranca sin medir
            return;
        }
        _processStart = Process.GetCurrentProcess().StartTime;
        _stallMilliseconds = configuration.GetValue("Diagnostics:StallMilliseconds", 150);
        foreach (var (at, phase) in Early) MarkAt(at - _processStart, phase);
        Mark("App.OnStartup");
        StartWatchdog(TimeSpan.FromSeconds(configuration.GetValue("Diagnostics:WatchSeconds", 60)));
    }

    public static void Mark(string phase) => MarkAt(DateTime.Now - _processStart, phase);

    private static void MarkAt(TimeSpan now, string phase)
    {
        if (_log is null) return;
        lock (Gate)
        {
            _log.WriteLine($"{now.TotalMilliseconds,8:0} ms  +{(now - _last).TotalMilliseconds,6:0} ms  {phase}");
            _last = now;
        }
    }

    public static void Measure(string phase, Action work) => Measure(phase, () =>
    {
        work();
        return true;
    });

    public static T Measure<T>(string phase, Func<T> work)
    {
        if (_log is null) return work();
        var watch = Stopwatch.StartNew();
        var result = work();
        Mark($"{phase} [{watch.ElapsedMilliseconds} ms]");
        return result;
    }

    public static async Task MeasureAsync(string phase, Func<Task> work)
    {
        if (_log is null)
        {
            await work();
            return;
        }
        var watch = Stopwatch.StartNew();
        await work();
        Mark($"{phase} [{watch.ElapsedMilliseconds} ms]");
    }

    // Latido cada 50 ms en la UI: si tarda más de StallMilliseconds, el hilo estuvo bloqueado
    private static void StartWatchdog(TimeSpan duration)
    {
        _lastBeat = Environment.TickCount64;
        var until = _lastBeat + (long)duration.TotalMilliseconds;
        _watchdog = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(50) };
        _watchdog.Tick += (_, _) =>
        {
            var now = Environment.TickCount64;
            var gap = now - _lastBeat;
            if (gap > _stallMilliseconds) Mark($"UI trabada {gap} ms");
            _lastBeat = now;
            if (now > until) _watchdog.Stop();
        };
        _watchdog.Start();
    }
}
