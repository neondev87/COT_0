// DESCARGAR PDF y ABRIR PDF de un ticket con su aviso listo (ruta en el Escritorio, abierto o el error)
using COT_0.Host.Models;
using COT_0.Host.Services.Tienda;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// La usan el panel COMPRA REALIZADA, TICKETS, el PUNTO DE VENTA y VER FACTURA. DESCARGAR copia al Escritorio
/// (TicketDownloads); ABRIR usa la copia de la app (TicketFiles, la genera si falta) con el visor de Windows.
/// </summary>
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

    public static async Task<(string Message, InfoBarSeverity Severity)> ViewAsync(PurchaseTicket ticket)
    {
        var number = TicketLines.Number(ticket.Number);
        try
        {
            if (await TicketViewer.OpenAsync(await TicketFiles.EnsureAsync(ticket)))
                return (StoreTexts.Format("Text.Store.PdfOpened", number), InfoBarSeverity.Success);
        }
        catch (Exception ex) when (TicketDownloads.IsSaveError(ex))
        {
            // Sin copia de la app no hay nada que abrir: cae al mismo aviso
        }
        return (StoreTexts.Format("Text.Store.PdfOpenFailed", number), InfoBarSeverity.Error);
    }
}
