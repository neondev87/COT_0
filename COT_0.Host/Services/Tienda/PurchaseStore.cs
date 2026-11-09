// Lee y guarda compras.json (%LocalAppData%\COT_0) con JsonStore; si está dañado lo recrea vacío y lo avisa
using System.Text.Json;
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Tienda;

/// <summary>Valida al leer y al guardar (PurchasesValidator); un JSON que no se puede leer no tumba la app.</summary>
public class PurchaseStore
{
    private static string FileName => AppConfig.Settings.Storage.PurchasesFile;

    /// <summary>true si la última lectura encontró el archivo dañado y lo recreó vacío (se apaga al leerlo).</summary>
    public bool WasRecreated { get; private set; }

    public async Task<PurchasesData> LoadAsync()
    {
        PurchasesData data;
        try
        {
            data = await JsonStore.LoadAsync<PurchasesData>(FileName);
        }
        catch (JsonException)
        {
            data = new PurchasesData();
            await SaveAsync(data);
            WasRecreated = true;
        }
        return PurchasesValidator.Clean(data);
    }

    public Task SaveAsync(PurchasesData data) => JsonStore.SaveAsync(FileName, PurchasesValidator.Clean(data));

    /// <summary>El aviso de "se recreó" sale una sola vez.</summary>
    public bool TakeRecreatedNotice()
    {
        var recreated = WasRecreated;
        WasRecreated = false;
        return recreated;
    }
}
