// Escenario del manual: pone la vista de ejemplo de cada paso y le pasa la fase para sus resaltados
using COT_0.HostWinUI.Services.Configuracion;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace COT_0.HostWinUI.Views.Componentes;

/// <summary>
/// Reemplaza al ContentControl con DataTrigger por paso del WPF (ManualStepView + ManualStages).
/// Solo existe la vista del paso actual; los pasos 3 y 4 comparten la de la sala. Nada adentro toma el foco,
/// así ENTER y las flechas siguen siendo del manual.
/// </summary>
public sealed partial class VistaEjemploManual : UserControl
{
    public static readonly DependencyProperty StepProperty = DependencyProperty.Register(
        nameof(Step), typeof(int), typeof(VistaEjemploManual), new PropertyMetadata(0, (d, _) => ((VistaEjemploManual)d).ShowStep()));

    public static readonly DependencyProperty PhaseProperty = DependencyProperty.Register(
        nameof(Phase), typeof(int), typeof(VistaEjemploManual), new PropertyMetadata(0, (d, _) => ((VistaEjemploManual)d).ShowPhase()));

    public VistaEjemploManual()
    {
        InitializeComponent();
        Stage.Width = AppConfig.Settings.Window.Width;
        Stage.Height = AppConfig.Settings.Window.Height;
        GettingFocus += OnGettingFocus;
    }

    public int Step
    {
        get => (int)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public int Phase
    {
        get => (int)GetValue(PhaseProperty);
        set => SetValue(PhaseProperty, value);
    }

    private VistaEjemplo? Current => Stage.Child as VistaEjemplo;

    private void ShowStep()
    {
        VistaEjemplo view = Step switch
        {
            1 => Current as VistaEjemploLogin ?? new VistaEjemploLogin(),
            2 => Current as VistaEjemploInicio ?? new VistaEjemploInicio(),
            3 or 4 => Current as VistaEjemploSala ?? new VistaEjemploSala(),
            5 => Current as VistaEjemploEmotes ?? new VistaEjemploEmotes(),
            _ => Current as VistaEjemploMusica ?? new VistaEjemploMusica(),
        };
        view.Phase = Phase;
        view.Step = Step;
        Stage.Child = view;
    }

    private void ShowPhase()
    {
        if (Current is { } view) view.Phase = Phase;
    }

    private static void OnGettingFocus(UIElement sender, GettingFocusEventArgs args) => args.TryCancel();
}
