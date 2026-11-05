using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.ViewModels;

// Notificación de Lain: visible entre las fases From y To del paso
public partial class ManualNoticeViewModel(string text, int from, int to) : ObservableObject
{
    [ObservableProperty]
    private bool _isVisible;

    public string Text { get; } = text;

    // true si recién apareció (para el sonido)
    public bool Apply(int phase)
    {
        var wasVisible = IsVisible;
        IsVisible = from <= phase && phase <= to;
        return IsVisible && !wasVisible;
    }
}
