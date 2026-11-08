// Notificación de Lain: visible entre las fases From y To del paso
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Aviso de Lain dentro de un paso; la vista lo anima con OpacityTransition.
/// </summary>
public partial class ManualNoticeViewModel(string text, int from, int to) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Opacity))]
    private bool _isVisible;

    public string Text { get; } = text;

    public double Opacity => IsVisible ? 1 : 0;

    /// <summary>true si recién apareció (para el sonido).</summary>
    public bool Apply(int phase)
    {
        var wasVisible = IsVisible;
        IsVisible = from <= phase && phase <= to;
        return IsVisible && !wasVisible;
    }
}
