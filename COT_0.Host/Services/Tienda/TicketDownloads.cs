// Guarda el ticket en PDF en el Escritorio del usuario de Windows, carpeta Store:TicketsFolder (ticket-0001.pdf)
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Tienda;

/// <summary>Si el PDF de ese ticket ya existe, no lo vuelve a generar; regresa la ruta final para el aviso.</summary>
public static class TicketDownloads
{
    private static StoreSettings Store => AppConfig.Settings.Store;

    public static string Folder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Store.TicketsFolder);

    public static string PathFor(int number) => Path.Combine(Folder, string.Format(Store.TicketFileFormat, number));

    public static async Task<string> SaveAsync(PurchaseTicket ticket)
    {
        var path = PathFor(ticket.Number);
        if (File.Exists(path)) return path;

        // Los textos se leen aquí (hilo de la UI); armar el PDF y escribirlo va aparte
        var lines = TicketLines.For(ticket);
        var folder = Folder;
        await Task.Run(() => Directory.CreateDirectory(folder));
        using var document = await Task.Run(() => TicketDocument.Render(lines));
        await using var file = AsyncFiles.Create(path);
        await document.SaveAsync(file, false);
        return path;
    }

    /// <summary>true si falló crear la carpeta, escribir el archivo o la fuente de Store:TicketFont (PDFsharp no la encontró).</summary>
    public static bool IsSaveError(Exception ex) => ex is IOException or UnauthorizedAccessException or InvalidOperationException;
}
