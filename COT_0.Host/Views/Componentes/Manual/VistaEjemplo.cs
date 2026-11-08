// Base de las vistas de ejemplo del manual: paso y fase actuales, y los datos ficticios
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes.Manual;

/// <summary>
/// Cada vista de ejemplo (Login, Inicio, Sala, Emotes, Música) hereda de aquí y enlaza sus ResaltadoManual
/// a Step y Phase con x:Bind. Las pone VistaEjemploManual; no se tocan ni toman el foco.
/// </summary>
public partial class VistaEjemplo : UserControl
{
    public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register(nameof(Step), typeof(int), typeof(VistaEjemplo), new PropertyMetadata(0));

    public static readonly DependencyProperty PhaseProperty =
        DependencyProperty.Register(nameof(Phase), typeof(int), typeof(VistaEjemplo), new PropertyMetadata(0));

    public VistaEjemplo()
    {
        IsHitTestVisible = false;
        IsTabStop = false;
    }

    public ManualDemoViewModel Demo { get; } = App.Services.GetRequiredService<ManualDemoViewModel>();

    /// <summary>Paso del manual que se está mostrando (la sala sirve a dos pasos).</summary>
    public int Step
    {
        get => (int)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    /// <summary>Fase del paso (ManualTimeline): la 2i+1 trae el aviso i y su resaltado.</summary>
    public int Phase
    {
        get => (int)GetValue(PhaseProperty);
        set => SetValue(PhaseProperty, value);
    }
}
