using System.Windows;
using System.Windows.Media;

namespace COT_0.Host.Controls;

// Paralelogramo que ocupa todo su espacio: el corte de Slant px se mantiene sin importar el ancho
public class SlantedShape : FrameworkElement
{
    public static readonly DependencyProperty SlantProperty = DependencyProperty.Register(
        nameof(Slant), typeof(double), typeof(SlantedShape),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
        nameof(Fill), typeof(Brush), typeof(SlantedShape),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
        nameof(Stroke), typeof(Brush), typeof(SlantedShape),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
        nameof(StrokeThickness), typeof(double), typeof(SlantedShape),
        new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

    public double Slant { get => (double)GetValue(SlantProperty); set => SetValue(SlantProperty, value); }
    public Brush? Fill { get => (Brush?)GetValue(FillProperty); set => SetValue(FillProperty, value); }
    public Brush? Stroke { get => (Brush?)GetValue(StrokeProperty); set => SetValue(StrokeProperty, value); }
    public double StrokeThickness { get => (double)GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }

    protected override void OnRender(DrawingContext drawingContext)
    {
        // El trazo va hacia adentro: se achica la forma medio grosor por lado
        var pen = Stroke is null ? null : new Pen(Stroke, StrokeThickness);
        var inset = pen is null ? 0 : StrokeThickness / 2;
        drawingContext.DrawGeometry(Fill, pen, Build(inset));
    }

    private StreamGeometry Build(double inset)
    {
        var (width, height) = (RenderSize.Width - inset, RenderSize.Height - inset);
        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            context.BeginFigure(new Point(Slant + inset, inset), true, true);
            context.PolyLineTo([new Point(width, inset), new Point(width - Slant, height), new Point(inset, height)], true, false);
        }
        geometry.Freeze();
        return geometry;
    }
}
