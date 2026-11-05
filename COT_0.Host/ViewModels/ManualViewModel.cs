using System.Diagnostics;
using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.ViewModels;

// Manual de Lain: seis pasos con notificaciones en el tiempo, atajos (ENTER, flechas, ESC) y despedida
public partial class ManualViewModel : ObservableObject, IDisposable
{
    private readonly ManualSound _sound;
    private readonly double[] _cueSeconds;
    private readonly TimeSpan _stepChange;
    private CancellationTokenSource? _timeline;
    private bool _isClosed;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Current))]
    [NotifyCanExecuteChangedFor(nameof(NextCommand), nameof(PreviousCommand))]
    private int _index;

    [ObservableProperty]
    private int _phase;

    [ObservableProperty]
    private bool _isLeaving;

    [ObservableProperty]
    private bool _isFarewell;

    [ObservableProperty]
    private bool _isMuted;

    public ManualViewModel(IConfiguration configuration, ManualSound sound, ManualAssets assets, ManualDemoFactory demo)
    {
        _sound = sound;
        _cueSeconds = configuration.GetSection("Manual:CueSeconds").Get<double[]>() ?? [];
        _stepChange = TimeSpan.FromMilliseconds(configuration.GetValue<double>("Manual:StepChangeMilliseconds"));
        LainVideo = assets.LainVideo;
        FarewellVideo = assets.FarewellVideo;
        Demo = demo.Create();
        Steps = BuildSteps(configuration.GetSection("Texts:Manual"));
        RunTimeline();
    }

    public event Action? CloseRequested;

    public IReadOnlyList<ManualStepViewModel> Steps { get; }
    public ManualStepViewModel Current => Steps[Index];
    public ManualDemo Demo { get; }
    public Uri? LainVideo { get; }
    public Uri? FarewellVideo { get; }

    // ENTER: siguiente paso; después del último, la despedida. En la despedida cierra
    [RelayCommand]
    private Task StartAsync()
    {
        if (!IsFarewell && Index < Steps.Count - 1) return GoAsync(Index + 1);
        Exit();
        return Task.CompletedTask;
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private Task NextAsync() => GoAsync(Index + 1);

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private Task PreviousAsync() => GoAsync(Index - 1);

    // ESC: en los pasos abre la despedida; en la despedida cierra
    [RelayCommand]
    private void Exit()
    {
        if (IsFarewell) Close();
        else Farewell();
    }

    [RelayCommand]
    private void Farewell()
    {
        if (IsFarewell) return;
        _timeline?.Cancel();
        IsFarewell = true;
        _sound.StartStatic();
    }

    [RelayCommand]
    private void Close()
    {
        if (_isClosed) return;
        _isClosed = true;
        CloseRequested?.Invoke();
    }

    [RelayCommand]
    private void ToggleMute() => IsMuted = !IsMuted;

    public void Dispose()
    {
        _timeline?.Cancel();
        _sound.Stop();
        GC.SuppressFinalize(this);
    }

    partial void OnIsMutedChanged(bool value) => _sound.IsMuted = value;

    partial void OnPhaseChanged(int value)
    {
        if (Current.Apply(value)) _sound.PlayNotice();
    }

    private bool CanGoNext() => Index < Steps.Count - 1;

    private bool CanGoBack() => Index > 0;

    // Sale el paso actual (fundido corto) y entra el nuevo desde la fase 0
    private async Task GoAsync(int index)
    {
        if (IsLeaving || IsFarewell || index < 0 || index >= Steps.Count || index == Index) return;
        _timeline?.Cancel();
        IsLeaving = true;
        await Task.Delay(_stepChange);
        // Fase 0 antes de cambiar: los recuadros del paso nuevo arrancan ocultos
        Phase = 0;
        Index = index;
        IsLeaving = false;
        RunTimeline();
    }

    // Avanza las fases a los segundos de Manual:CueSeconds; se cancela al cambiar de paso
    private async void RunTimeline()
    {
        var timeline = _timeline = new CancellationTokenSource();
        Phase = 0;
        Current.Apply(0);
        var clock = Stopwatch.StartNew();
        try
        {
            for (var phase = 1; phase <= Current.LastPhase && phase <= _cueSeconds.Length; phase++)
            {
                var wait = TimeSpan.FromSeconds(_cueSeconds[phase - 1]) - clock.Elapsed;
                if (wait > TimeSpan.Zero) await Task.Delay(wait, timeline.Token);
                Phase = phase;
            }
        }
        catch (TaskCanceledException)
        {
        }
    }

    private static List<ManualStepViewModel> BuildSteps(IConfigurationSection texts)
    {
        var steps = texts.GetSection("Steps").GetChildren().ToList();
        var format = texts["StepFormat"] ?? "{0}";
        return steps.Select((step, i) => new ManualStepViewModel(
                i + 1,
                string.Format(format, i + 1, steps.Count),
                step["Title"] ?? string.Empty,
                step["Text"] ?? string.Empty,
                step.GetSection("Notices").Get<string[]>() ?? []))
            .ToList();
    }
}
