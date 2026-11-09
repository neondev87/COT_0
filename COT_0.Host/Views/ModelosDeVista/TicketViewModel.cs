// Un ticket en la sección TICKETS: número, fecha, hora, miniaturas de los emotes y DESCARGAR PDF
using System.Globalization;
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Tienda;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Fecha y hora en la hora local de la computadora (en compras.json van en UTC).</summary>
public sealed class TicketViewModel(PurchaseTicket ticket)
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo(AppConfig.Settings.Ui.Culture);

    public PurchaseTicket Ticket { get; } = ticket;

    public string Number { get; } = StoreTexts.Format("Text.Store.Tickets.Number", TicketLines.Number(ticket.Number));

    public string Date { get; } = ticket.CreatedAt.ToLocalTime().ToString(AppConfig.Settings.Ui.DateFormat, Culture).ToUpper(Culture);

    public string Time { get; } = ticket.CreatedAt.ToLocalTime().ToString(AppConfig.Settings.Ui.ClockFormat, Culture);

    public string Detail { get; } = StoreTexts.Format("Text.Store.Tickets.Detail", ticket.Emotes.Count, ticket.Remaining, AppConfig.Settings.Store.DailyLimit);

    public IReadOnlyList<EmoteThumbViewModel> Emotes { get; } =
        ticket.Emotes.Select(e => new EmoteThumbViewModel(e.Name, EmoteMapping.ThumbnailUrl(e))).ToList();
}
