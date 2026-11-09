// Arma los renglones del ticket (textos de Textos.xaml y fechas en hora local) para el PDF
using System.Globalization;
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Textos;

namespace COT_0.Host.Services.Tienda;

/// <summary>
/// Se llama en el hilo de la UI (los textos salen de los recursos de la app); el dibujo va aparte en TicketDocument.
/// No pone el nombre del usuario: el ticket es solo de la compra.
/// </summary>
public static class TicketLines
{
    // Courier New a 9 pt en 80 mm: caben unos 38 caracteres por renglón
    private const int MaxNameLength = 30;

    private static StoreSettings Store => AppConfig.Settings.Store;

    public static string Number(int number) => string.Format(CultureInfo.InvariantCulture, Store.TicketNumberFormat, number);

    public static string LocalDate(DateTime utc) =>
        utc.ToLocalTime().ToString(Store.TicketDateFormat, CultureInfo.GetCultureInfo(AppConfig.Settings.Ui.Culture));

    public static IReadOnlyList<TicketLine> For(PurchaseTicket ticket)
    {
        var culture = CultureInfo.CurrentCulture;
        var lines = new List<TicketLine>
        {
            new(UiText.Get("Text.Ticket.Title"), TicketLineKind.Title),
            new(UiText.Get("Text.Ticket.Subtitle")),
            new(string.Empty, TicketLineKind.Rule),
            new(string.Format(culture, UiText.Get("Text.Ticket.Number"), Number(ticket.Number)), TicketLineKind.Bold),
            new(string.Format(culture, UiText.Get("Text.Ticket.Date"), LocalDate(ticket.CreatedAt))),
            new(string.Empty, TicketLineKind.Rule),
            new(UiText.Get("Text.Ticket.Emotes"), TicketLineKind.Bold),
        };
        lines.AddRange(ticket.Emotes.Select(e => new TicketLine(string.Format(culture, UiText.Get("Text.Ticket.EmoteLine"), Shorten(e.Name)))));
        lines.Add(new TicketLine(string.Empty, TicketLineKind.Rule));
        lines.Add(new TicketLine(string.Format(culture, UiText.Get("Text.Ticket.Total"), ticket.Emotes.Count), TicketLineKind.Bold));
        lines.Add(new TicketLine(string.Format(culture, UiText.Get("Text.Ticket.Remaining"), ticket.Remaining, Store.DailyLimit)));
        lines.Add(new TicketLine(string.Format(culture, UiText.Get("Text.Ticket.Resets"), LocalDate(ticket.ResetsAt))));
        lines.Add(new TicketLine(string.Empty, TicketLineKind.Rule));
        lines.Add(new TicketLine(UiText.Get("Text.Ticket.Footer")));
        return lines;
    }

    private static string Shorten(string name) => name.Length <= MaxNameLength ? name : name[..(MaxNameLength - 1)] + "…";
}
