// VER FACTURA (modo ADMIN, solo lectura): los tickets de un usuario con DESCARGAR PDF y ABRIR PDF
using COT_0.Host.Models;
using COT_0.Host.Services.Tienda;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Se arma con lo leído de compras.json (del más nuevo al más viejo). El total en dólares es de muestra
/// (EmotePricing): no hay cobro. Aquí no se compra ni se borra nada.
/// </summary>
public sealed partial class UserInvoicesViewModel
{
    public UserInvoicesViewModel(string userName, UserPurchases purchases)
    {
        Title = StoreTexts.Format("Text.Invoices.Title", userName.ToUpperInvariant());
        Tickets = purchases.Tickets
            .OrderByDescending(t => t.Number)
            .Select(t => new TicketViewModel(t, userName))
            .ToList();
        var emotes = purchases.Tickets.SelectMany(t => t.Emotes).ToList();
        Summary = StoreTexts.Format("Text.Invoices.Summary", Tickets.Count, emotes.Count,
            EmotePricing.Format(EmotePricing.TotalOf(emotes.Select(e => e.Id))));
    }

    public string Title { get; }

    public string Summary { get; }

    public IReadOnlyList<TicketViewModel> Tickets { get; }

    public bool IsEmpty => Tickets.Count == 0;

    public bool HasTickets => !IsEmpty;

    public NoticeViewModel Notice { get; } = new();

    [RelayCommand]
    private async Task DownloadAsync(TicketViewModel item)
    {
        var (message, severity) = await TicketPdfActions.DownloadAsync(item.Ticket);
        Notice.Show(message, severity);
    }

    [RelayCommand]
    private async Task ViewAsync(TicketViewModel item)
    {
        var (message, severity) = await TicketPdfActions.ViewAsync(item.Ticket);
        Notice.Show(message, severity);
    }
}
