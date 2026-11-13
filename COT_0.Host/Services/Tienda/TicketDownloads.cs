// Copia el ticket en PDF al Escritorio del usuario de Windows, carpeta Store:TicketsFolder (ticket-0001.pdf)
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Tienda;

/// <summary>
/// Parte de la copia que genera TicketFiles (la crea si falta). Si el PDF ya está en el Escritorio, no lo vuelve
/// a copiar; regresa la ruta final para el aviso.
/// </summary>
public static class TicketDownloads
{
    private static StoreSettings Store => AppConfig.Settings.Store;

    public static string Folder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Store.TicketsFolder);

    public static string PathFor(int number) => Path.Combine(Folder, string.Format(Store.TicketFileFormat, number));

    public static async Task<string> SaveAsync(PurchaseTicket ticket)
    {
        var path = PathFor(ticket.Number);
        if (File.Exists(path)) return path;

        var source = await TicketFiles.EnsureAsync(ticket);
        var folder = Folder;
        await Task.Run(() => Directory.CreateDirectory(folder));
        await using var input = AsyncFiles.OpenRead(source);
        await using var output = AsyncFiles.Create(path);
        await input.CopyToAsync(output);
        return path;
    }

    /// <summary>true si falló crear la carpeta, escribir el archivo o la fuente de Store:TicketFont (PDFsharp no la encontró).</summary>
    public static bool IsSaveError(Exception ex) => ex is IOException or UnauthorizedAccessException or InvalidOperationException;
}
