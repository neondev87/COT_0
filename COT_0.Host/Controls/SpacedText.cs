using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace COT_0.Host.Controls;

// Texto de una línea con letter-spacing (en em, como el CSS del diseño) y line-height opcional
public class SpacedText : FrameworkElement
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(SpacedText),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
        nameof(Spacing), typeof(double), typeof(SpacedText),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty LineHeightProperty = DependencyProperty.Register(
        nameof(LineHeight), typeof(double), typeof(SpacedText),
        new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(SpacedText),
        new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(SpacedText),
        new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(SpacedText),
        new FrameworkPropertyMetadata(FontWeights.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(SpacedText),
        new FrameworkPropertyMetadata(FontStyles.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(SpacedText),
        new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public double Spacing { get => (double)GetValue(SpacingProperty); set => SetValue(SpacingProperty, value); }
    public double LineHeight { get => (double)GetValue(LineHeightProperty); set => SetValue(LineHeightProperty, value); }
    public FontFamily FontFamily { get => (FontFamily)GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }
    public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
    public FontWeight FontWeight { get => (FontWeight)GetValue(FontWeightProperty); set => SetValue(FontWeightProperty, value); }
    public FontStyle FontStyle { get => (FontStyle)GetValue(FontStyleProperty); set => SetValue(FontStyleProperty, value); }
    public Brush Foreground { get => (Brush)GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }

    private double Gap => Spacing * FontSize;

    protected override Size MeasureOverride(Size availableSize)
    {
        var glyphs = BuildGlyphs();
        var width = glyphs.Sum(g => g.WidthIncludingTrailingWhitespace + Gap);
        var height = double.IsNaN(LineHeight) ? TextHeight(glyphs) : LineHeight;
        return new Size(Math.Max(0, width), height);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        var glyphs = BuildGlyphs();
        if (glyphs.Count == 0) return;

        // Como en CSS: el espacio extra del line-height se reparte arriba y abajo
        var top = (RenderSize.Height - TextHeight(glyphs)) / 2;
        var x = 0d;
        foreach (var glyph in glyphs)
        {
            drawingContext.DrawText(glyph, new Point(x, top));
            x += glyph.WidthIncludingTrailingWhitespace + Gap;
        }
    }

    private double TextHeight(List<FormattedText> glyphs) =>
        glyphs.Count > 0 ? glyphs.Max(g => g.Height) : FontSize;

    private List<FormattedText> BuildGlyphs()
    {
        var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal);
        var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
        return (Text ?? string.Empty)
            .Select(c => new FormattedText(c.ToString(), CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                typeface, FontSize, Foreground, pixelsPerDip))
            .ToList();
    }
}
