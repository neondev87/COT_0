// Manual de Lain: seis pasos con notificaciones en el tiempo, atajos (ENTER, flechas, ESC) y despedida
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Fondos;
using COT_0.HostWinUI.Services.Manual;
using COT_0.HostWinUI.Services.Textos;
using COT_0.HostWinUI.Services.Ventana;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Windows.Media.Playback;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>
/// Misma lógica que el ManualViewModel del WPF (pasos, fases, despedida, silencio) pero como página:
/// EnterAsync al cargar y Leave al salir. Phase llega a la vista de ejemplo para sus resaltados.
/// Opacidades en lugar de DataTrigger; FlashOpacity es el destello
/// corto que sustituye a la pérdida de señal (SignalLossLayer). Archivos que faltan: aviso en InfoBar.
/// </summary>
public partial class ManualViewModel : ObservableObject
{
    private readonly ManualAssets _assets;
    private readonly ManualSound _sound;
    private readonly ManualVideo _video;
    private readonly ManualTimeline _timeline;
    private readonly FondoImages _images;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Current))]
    [NotifyCanExecuteChangedFor(nameof(NextCommand), nameof(PreviousCommand))]
    private int _index;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StepOpacity))]
    private bool _isLeaving;

    [ObservableProperty]
    private bool _isFarewell;

    [ObservableProperty]
    private bool _isMuted;

    [ObservableProperty]
    private double _flashOpacity;

    [ObservableProperty]
    private int _phase;

    [ObservableProperty]
    private bool _isErrorOpen;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ManualViewModel(ManualAssets assets, ManualSound sound, ManualVideo video, ManualTimeline timeline, FondoImages images)
    {
        _assets = assets;
        _sound = sound;
        _video = video;
        _timeline = timeline;
        _images = images;
        Steps = ManualStepViewModel.FromTexts();
        timeline.PhaseChanged += OnTimelinePhase;
    }

    public IReadOnlyList<ManualStepViewModel> Steps { get; }

    public ManualStepViewModel Current => Steps[Index];

    public double StepOpacity => IsLeaving ? 0 : 1;

    public MediaPlayer VideoPlayer => _video.Player;

    /// <summary>Al cargar la página: revisa los archivos, arranca a Lain y el primer paso.</summary>
    public async Task EnterAsync()
    {
        _video.Failed += OnVideoFailed;
        await _assets.RefreshAsync();
        if (_assets.Missing.Count > 0) ShowError("Text.Manual.Error.Missing", string.Join(", ", _assets.Missing.Select(Path.GetFileName)));
        _sound.Open();
        _video.Play(_assets.LainVideo);
        _timeline.Start(Current.LastPhase);
        await LoadStagesAsync();
    }

    /// <summary>Al salir de la página: corta el reloj, el video y los sonidos.</summary>
    public void Leave()
    {
        _video.Failed -= OnVideoFailed;
        _timeline.Cancel();
        _sound.Stop();
        _video.Stop();
    }

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
        _timeline.Cancel();
        IsFarewell = true;
        _sound.StartStatic();
        _video.Play(_assets.FarewellVideo);
    }

    [RelayCommand]
    private void Close() => Navigator.ShowDashboard();

    [RelayCommand]
    private void ToggleMute() => IsMuted = !IsMuted;

    partial void OnIsMutedChanged(bool value) => _sound.IsMuted = value;

    private void OnTimelinePhase(int phase)
    {
        Phase = phase;
        if (Current.Apply(phase)) _sound.PlayNotice();
    }

    private bool CanGoNext() => Index < Steps.Count - 1;

    private bool CanGoBack() => Index > 0;

    // Sale el paso actual con un destello corto y entra el nuevo desde la fase 0
    private async Task GoAsync(int index)
    {
        if (IsLeaving || IsFarewell || index < 0 || index >= Steps.Count || index == Index) return;
        _timeline.Cancel();
        IsLeaving = true;
        FlashOpacity = 1;
        await Task.Delay(AppConfig.Settings.Manual.StepChangeMilliseconds);
        Current.Apply(0);
        Phase = 0;
        Index = index;
        IsLeaving = false;
        FlashOpacity = 0;
        _timeline.Start(Current.LastPhase);
    }

    // Capturas opcionales: un paso sin la suya muestra su vista de ejemplo
    private async Task LoadStagesAsync()
    {
        foreach (var step in Steps.Where(s => s.Stage is null))
        {
            if (await _assets.FindStageAsync(step.Number) is not { } path) continue;
            step.Stage = await _images.LoadAsync(path, FondoKind.Image, AppConfig.Settings.Manual.StageDecodeSize);
            if (step.Stage is null) ShowError("Text.Manual.Error.Unreadable", Path.GetFileName(path));
        }
    }

    private void OnVideoFailed(string path) => ShowError("Text.Manual.Error.Unreadable", Path.GetFileName(path));

    private void ShowError(string key, string detail)
    {
        ErrorMessage = string.Format(UiText.Get(key), detail);
        IsErrorOpen = true;
    }
}
