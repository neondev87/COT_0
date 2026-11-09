// Lee y guarda admin.json (%LocalAppData%\COT_0) con JsonStore; si está dañado lo recrea vacío y lo avisa
using System.Text.Json;
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Administradores;

/// <summary>
/// Al leer descarta entradas sin nombre o sin hash y nombres repetidos. Un admin.json dañado se recrea vacío:
/// la app vuelve a pedir el primer admin (igual que si lo borras a mano para restablecerlo).
/// </summary>
public class AdminStore
{
    private static string FileName => AppConfig.Settings.Storage.AdminsFile;

    private bool _wasRecreated;

    public async Task<List<AdminAccount>> LoadAsync()
    {
        List<AdminAccount> admins;
        try
        {
            admins = await JsonStore.LoadAsync<List<AdminAccount>>(FileName);
        }
        catch (JsonException)
        {
            admins = [];
            await SaveAsync(admins);
            _wasRecreated = true;
        }
        return Clean(admins);
    }

    public Task SaveAsync(List<AdminAccount> admins) => JsonStore.SaveAsync(FileName, Clean(admins));

    /// <summary>true una sola vez si admin.json estaba dañado y se recreó vacío.</summary>
    public bool TakeRecreatedNotice()
    {
        var recreated = _wasRecreated;
        _wasRecreated = false;
        return recreated;
    }

    private static List<AdminAccount> Clean(List<AdminAccount> admins) => admins
        .Where(a => a is not null && !string.IsNullOrWhiteSpace(a.UserName)
            && !string.IsNullOrEmpty(a.PasswordHash) && !string.IsNullOrEmpty(a.Salt))
        .DistinctBy(a => a.UserName, StringComparer.OrdinalIgnoreCase)
        .ToList();
}
