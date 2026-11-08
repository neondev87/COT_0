// Reloj de cada paso del manual: avanza las fases en los segundos de Manual:CueSeconds
using System.Diagnostics;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Manual;

/// <summary>
/// Fase 0 = sin avisos; la fase 2i+1 muestra el aviso i
/// y la 2i+2 lo quita. Se arranca desde el hilo de la UI, así que PhaseChanged llega ahí mismo.
/// </summary>
public sealed class ManualTimeline
{
    private CancellationTokenSource? _run;

    public event Action<int>? PhaseChanged;

    public void Start(int lastPhase)
    {
        Cancel();
        _ = RunAsync(lastPhase, (_run = new CancellationTokenSource()).Token);
    }

    public void Cancel()
    {
        _run?.Cancel();
        _run?.Dispose();
        _run = null;
    }

    private async Task RunAsync(int lastPhase, CancellationToken token)
    {
        var cues = AppConfig.Settings.Manual.CueSeconds;
        PhaseChanged?.Invoke(0);
        var clock = Stopwatch.StartNew();
        try
        {
            for (var phase = 1; phase <= lastPhase && phase <= cues.Count; phase++)
            {
                var wait = TimeSpan.FromSeconds(cues[phase - 1]) - clock.Elapsed;
                if (wait > TimeSpan.Zero) await Task.Delay(wait, token);
                PhaseChanged?.Invoke(phase);
            }
        }
        catch (TaskCanceledException)
        {
        }
    }
}
