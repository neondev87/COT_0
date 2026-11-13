// DESCARGAR PDF de un ticket con su aviso listo (ruta en el Escritorio o el error)
using COT_0.Host.Models;
using COT_0.Host.Services.Tienda;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>La usan el panel COMPRA REALIZADA, TICKETS y el PUNTO DE VENTA; el PDF va al Escritorio (TicketDownloads).</summary>
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
}
