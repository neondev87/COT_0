// Dibuja el ticket en un PDF angosto (80 mm, como de caja) con PDFsharp; corre fuera del hilo de la UI
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;

namespace COT_0.Host.Services.Tienda;

/// <summary>Fondo blanco para que se pueda imprimir; el título lleva una línea con el acento del proyecto.</summary>
public static class TicketDocument
{
    private const double WidthMm = 80;
    private const double Margin = 18;
    private const double TitleSize = 22;
    private const double TextSize = 9;
    private const double LineHeight = 15;
    private const double TitleHeight = 32;
    private const double RuleHeight = 12;

    private static readonly XColor Accent = XColor.FromArgb(0xB3, 0x12, 0x2E);
    private static readonly XColor Ink = XColor.FromArgb(0x15, 0x12, 0x1B);
    private static readonly XColor Soft = XColor.FromArgb(0x9A, 0x96, 0xA0);

    // PDFsharp 6 (build Core) solo usa las fuentes de Windows si se le pide
    static TicketDocument() => GlobalFontSettings.UseWindowsFontsUnderWindows = true;

    public static PdfDocument Render(IReadOnlyList<TicketLine> lines)
    {
        var font = AppConfig.Settings.Store.TicketFont;
        var title = new XFont(font, TitleSize, XFontStyleEx.Bold);
        var text = new XFont(font, TextSize, XFontStyleEx.Regular);
        var bold = new XFont(font, TextSize, XFontStyleEx.Bold);

        var document = new PdfDocument();
        var page = document.AddPage();
        page.Width = XUnit.FromMillimeter(WidthMm);
        page.Height = XUnit.FromPoint(Margin * 2 + lines.Sum(Height));

        using var graphics = XGraphics.FromPdfPage(page);
        var width = page.Width.Point - Margin * 2;
        var y = Margin;
        foreach (var line in lines)
        {
            Draw(graphics, line, line.Kind switch { TicketLineKind.Title => title, TicketLineKind.Bold => bold, _ => text }, y, width);
            y += Height(line);
        }
        return document;
    }

    private static void Draw(XGraphics graphics, TicketLine line, XFont font, double y, double width)
    {
        if (line.Kind == TicketLineKind.Rule)
        {
            var pen = new XPen(Soft, 0.6) { DashStyle = XDashStyle.Dash };
            graphics.DrawLine(pen, Margin, y + RuleHeight / 2, Margin + width, y + RuleHeight / 2);
            return;
        }

        var box = new XRect(Margin, y, width, Height(line));
        graphics.DrawString(line.Text, font, new XSolidBrush(Ink), box, XStringFormats.TopCenter);
        if (line.Kind == TicketLineKind.Title)
            graphics.DrawLine(new XPen(Accent, 1.4), Margin + width / 2 - 18, y + TitleHeight - 4, Margin + width / 2 + 18, y + TitleHeight - 4);
    }

    private static double Height(TicketLine line) => line.Kind switch
    {
        TicketLineKind.Title => TitleHeight,
        TicketLineKind.Rule => RuleHeight,
        _ => LineHeight,
    };
}
