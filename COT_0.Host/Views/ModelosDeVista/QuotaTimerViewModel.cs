// Temporizador HH:MM:SS del cupo agotado: cuenta hacia la hora de reinicio guardada en compras.json
using Microsoft.UI.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// No lleva su propia cuenta: cada segundo resta la hora actual a ResetsAt (leído del JSON), así sigue bien aunque cierres
/// la app o la compu. Al llegar a 00:00:00 se detiene y avisa con Expired para que la tienda vuelva a leer el cupo.
/// La pantalla lo detiene al salir (Stop).
/// </summary>
public partial class QuotaTimerViewModel : ObservableObject
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private DateTime _resetsAt;
    private int _limit;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BlockedText))]
    private string _text = string.Empty;

    public QuotaTimerViewModel() => _timer.Tick += (_, _) => Update();

    public event Action? Expired;

    /// <summary>"Ya compraste tus 10 emotes. Podrás comprar más en HH:MM:SS" (InfoBar del bloqueo).</summary>
    public string BlockedText => StoreTexts.Blocked(_limit, Text);

    public void Start(DateTime resetsAtUtc, int limit)
    {
        _resetsAt = resetsAtUtc;
        _limit = limit;
        Update();
        if (_resetsAt > DateTime.UtcNow) _timer.Start();
    }

    public void Stop() => _timer.Stop();

    private void Update()
    {
        var wait = _resetsAt - DateTime.UtcNow;
        Text = StoreTexts.Wait(wait);
        if (wait > TimeSpan.Zero) return;

        _timer.Stop();
        Expired?.Invoke();
    }
}
