// Resaltado de un elemento de la vista de ejemplo: visible entre las fases From y To (y solo en su paso, si tiene)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

/// <summary>
/// Sin buscar elementos en el árbol: va junto al elemento.
/// Aparece y se va con OpacityTransition; el pulso solo corre mientras se ve.
/// </summary>
public sealed partial class ResaltadoManual : UserControl
{
    private const int AnyStep = 0;

    public static readonly DependencyProperty PhaseProperty = Register(nameof(Phase), 0);
    public static readonly DependencyProperty FromProperty = Register(nameof(From), 1);
    public static readonly DependencyProperty ToProperty = Register(nameof(To), int.MaxValue);
    public static readonly DependencyProperty StepProperty = Register(nameof(Step), AnyStep);
    public static readonly DependencyProperty OnlyInStepProperty = Register(nameof(OnlyInStep), AnyStep);

    private bool _isShown;

    public ResaltadoManual()
    {
        InitializeComponent();
        Loaded += (_, _) => { if (_isShown) PulseStoryboard.Begin(); };
        Unloaded += (_, _) => PulseStoryboard.Stop();
    }

    /// <summary>Fase actual del paso (la enlaza la vista de ejemplo).</summary>
    public int Phase
    {
        get => (int)GetValue(PhaseProperty);
        set => SetValue(PhaseProperty, value);
    }

    public int From
    {
        get => (int)GetValue(FromProperty);
        set => SetValue(FromProperty, value);
    }

    public int To
    {
        get => (int)GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    /// <summary>Paso que se está mostrando.</summary>
    public int Step
    {
        get => (int)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    /// <summary>Si la vista sirve a dos pasos, el paso de este resaltado; 0 = cualquiera.</summary>
    public int OnlyInStep
    {
        get => (int)GetValue(OnlyInStepProperty);
        set => SetValue(OnlyInStepProperty, value);
    }

    private static DependencyProperty Register(string name, int value) =>
        DependencyProperty.Register(name, typeof(int), typeof(ResaltadoManual),
            new PropertyMetadata(value, (d, _) => ((ResaltadoManual)d).Update()));

    private void Update()
    {
        var show = (OnlyInStep == AnyStep || OnlyInStep == Step) && From <= Phase && Phase <= To;
        if (show == _isShown) return;
        _isShown = show;
        Root.Opacity = show ? 1 : 0;
        if (!show) PulseStoryboard.Stop();
        else if (IsLoaded) PulseStoryboard.Begin();
    }
}
