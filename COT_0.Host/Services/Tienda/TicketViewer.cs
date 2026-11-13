// Abre el PDF de un ticket con el visor que tenga Windows (SOLO VER PDF): no lo copia a ningún lado
using Windows.Storage;
using Windows.System;

namespace COT_0.Host.Services.Tienda;

/// <summary>false si Windows no pudo abrirlo (sin visor de PDF o archivo inaccesible).</summary>
public static class TicketViewer
{
    public static async Task<bool> OpenAsync(string path)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            return await Launcher.LaunchFileAsync(file);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            return false;
        }
    }
}
