// Carpetas de caché de la app: %LocalAppData%\COT_0\cache\<nombre> (fuera del repo)
using System.Security.Cryptography;
using System.Text;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Cache;

/// <summary>
/// Lo generado (cuadros de video) se guarda aquí con un nombre que sale de la ruta, el tamaño y la fecha
/// del archivo original: si el archivo cambia, el nombre cambia y se vuelve a generar.
/// </summary>
public static class CacheFolders
{
    /// <summary>Crea la carpeta si falta: es síncrono, llámalo dentro de Task.Run (como FondoFrameCache).</summary>
    public static string Get(string name)
    {
        var folder = Path.Combine(JsonStore.Folder, AppConfig.Settings.Storage.CacheFolder, name);
        Directory.CreateDirectory(folder);
        return folder;
    }

    /// <summary>Hash de ruta + tamaño + fecha de modificación (hex, 32 caracteres).</summary>
    public static string KeyOf(FileInfo file)
    {
        var text = $"{file.FullName.ToUpperInvariant()}|{file.Length}|{file.LastWriteTimeUtc.Ticks}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)), 0, 16).ToLowerInvariant();
    }
}
