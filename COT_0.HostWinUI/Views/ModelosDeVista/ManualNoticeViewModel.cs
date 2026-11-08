// Notificación de Lain: visible entre las fases From y To del paso
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>
/// Copia del WPF. Opacity reemplaza al DataTrigger con Storyboard: la vista la anima con OpacityTransition.
/// RowOpacity marca en la lista del paso los avisos que ya salieron.
/// </summary>
public partial class ManualNoticeViewModel(string text, int from, int to) : ObservableObject
{
    private const double PendingRowOpacity = 0.35;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Opacity))]
    private bool _isVisible;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RowOpacity))]
    private bool _isReached;

    public string Text { get; } = text;

    public double Opacity => IsVisible ? 1 : 0;

    public double RowOpacity => IsReached ? 1 : PendingRowOpacity;

    /// <summary>true si recién apareció (para el sonido).</summary>
    public bool Apply(int phase)
    {
        var wasVisible = IsVisible;
        IsVisible = from <= phase && phase <= to;
        IsReached = phase >= from;
        return IsVisible && !wasVisible;
    }
}
