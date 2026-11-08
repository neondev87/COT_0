// Lee y guarda fondos.json (catálogo y selección) en %LocalAppData%\COT_0
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Almacenamiento;
using COT_0.HostWinUI.Services.Configuracion;

namespace COT_0.HostWinUI.Services.Fondos;

/// <summary>Todo async con JsonStore; los errores de disco los decide quien llama.</summary>
public class FondoStore
{
    private static string FileName => AppConfig.Settings.Storage.BackgroundsFile;

    public Task<FondosGuardados> LoadAsync() => JsonStore.LoadAsync<FondosGuardados>(FileName);

    public Task SaveAsync(FondosGuardados data) => JsonStore.SaveAsync(FileName, data);
}
