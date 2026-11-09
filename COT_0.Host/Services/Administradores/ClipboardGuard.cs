// Copia la contraseña generada al portapapeles y lo limpia a los Admin:ClipboardClearSeconds si sigue siendo ella
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using COT_0.Host.Services.Configuracion;
using Windows.ApplicationModel.DataTransfer;

namespace COT_0.Host.Services.Administradores;

/// <summary>
/// Sin historial del portapapeles ni sincronización entre equipos. Para comparar después no guarda la contraseña:
/// solo su SHA-256. Si para entonces copiaste otra cosa, no la toca. Se llama desde el hilo de la UI.
/// </summary>
public static class ClipboardGuard
{
    private static byte[]? _pendingHash;

    /// <summary>true si se pudo copiar.</summary>
    public static bool CopySecret(string secret)
    {
        try
        {
            var package = new DataPackage();
            package.SetText(secret);
            Clipboard.SetContentWithOptions(package, new ClipboardContentOptions { IsAllowedInHistory = false, IsRoamable = false });
        }
        catch (COMException)
        {
            return false;
        }

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        _pendingHash = hash;
        _ = ClearLaterAsync(hash);
        return true;
    }

    private static async Task ClearLaterAsync(byte[] hash)
    {
        await Task.Delay(TimeSpan.FromSeconds(AppConfig.Settings.Admin.ClipboardClearSeconds));
        // Otra copia más nueva ya programó su propia limpieza
        if (!ReferenceEquals(hash, _pendingHash)) return;
        _pendingHash = null;
        try
        {
            var content = Clipboard.GetContent();
            if (!content.Contains(StandardDataFormats.Text)) return;
            var current = await content.GetTextAsync();
            if (CryptographicOperations.FixedTimeEquals(SHA256.HashData(Encoding.UTF8.GetBytes(current)), hash)) Clipboard.Clear();
        }
        catch (COMException)
        {
            // Otra app tenía el portapapeles abierto; no se puede hacer más sin guardar la contraseña
        }
    }
}
