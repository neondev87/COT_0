// Genera el PDF de un ticket en %LocalAppData%\COT_0\{Store:GeneratedTicketsFolder} (siempre, al comprar) y lo borra con su cuenta
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Tienda;

/// <summary>
/// Es la copia de la app: SOLO VER PDF abre esta y DESCARGAR PDF la copia al Escritorio (TicketDownloads).
/// Si ya existe, no la vuelve a generar. Se llama desde el hilo de la UI (los textos salen de los recursos).
/// </summary>
public static class TicketFiles
{
    private static StoreSettings Store => AppConfig.Settings.Store;

    public static string Folder => Path.Combine(JsonStore.Folder, Store.GeneratedTicketsFolder);

    public static string PathFor(int number) => Path.Combine(Folder, string.Format(Store.TicketFileFormat, number));

    public static async Task<string> EnsureAsync(PurchaseTicket ticket)
    {
        var path = PathFor(ticket.Number);
        if (File.Exists(path)) return path;

        var lines = TicketLines.For(ticket);
        var folder = Folder;
        await Task.Run(() => Directory.CreateDirectory(folder));
        using var document = await Task.Run(() => TicketDocument.Render(lines));
        await using var file = AsyncFiles.Create(path);
        await document.SaveAsync(file, false);
        return path;
    }

    /// <summary>Borra las copias de la app de esos tickets (al eliminar una cuenta). Si alguna está abierta, se queda.</summary>
    public static Task DeleteAsync(IReadOnlyList<int> numbers) => Task.Run(() =>
    {
        foreach (var path in numbers.Select(PathFor))
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Abierta en el visor: no detiene la eliminación de la cuenta
            }
        }
    });
}
