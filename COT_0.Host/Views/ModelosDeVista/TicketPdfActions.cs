// DESCARGAR PDF de un ticket y la pregunta "¿Quieres descargar tu ticket en PDF?" al comprar, con su aviso listo
using COT_0.Host.Models;
using COT_0.Host.Services.Dialogos;
using COT_0.Host.Services.Tienda;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>La usan la TIENDA, TICKETS y el PUNTO DE VENTA; el PDF va al Escritorio (TicketDownloads) y el aviso dice la ruta.</summary>
public static class TicketPdfActions
{
    public static async Task<(string Message, InfoBarSeverity Severity)> DownloadAsync(PurchaseTicket ticket)
    {
        try
        {
            return (StoreTexts.Format("Text.Store.PdfSaved", await TicketDownloads.SaveAsync(ticket)), InfoBarSeverity.Success);
        }
        catch (Exception ex) when (TicketDownloads.IsSaveError(ex))
        {
            return (StoreTexts.Format("Text.Store.PdfFailed", TicketDownloads.Folder), InfoBarSeverity.Error);
        }
    }

    /// <summary>Sí: lo guarda en el Escritorio. No: el ticket queda solo en TICKETS y el aviso lo dice.</summary>
    public static async Task<(string Message, InfoBarSeverity Severity)> OfferAsync(ConfirmDialogService dialogs, PurchaseTicket ticket)
    {
        var texts = new ConfirmTexts(StoreTexts.Format("Text.Store.PdfDialog.Message"),
            StoreTexts.Format("Text.Store.PdfDialog.Confirm"), StoreTexts.Format("Text.Store.PdfDialog.Cancel"));
        return await dialogs.ConfirmAsync(texts)
            ? await DownloadAsync(ticket)
            : (StoreTexts.Format("Text.Store.Shop.Bought", ticket.Emotes.Count), InfoBarSeverity.Success);
    }
}
