// Bitácora de entradas a la sala en visitas.json (queda guardada aunque se cierre el túnel)
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Almacenamiento;
using COT_0.HostWinUI.Services.Configuracion;

namespace COT_0.HostWinUI.Services.Visitas;

/// <summary>Agrega un registro con la hora cada vez que alguien entra.</summary>
public static class VisitService
{
    // Dos invitados pueden entrar al mismo tiempo: una escritura a la vez
    private static readonly SemaphoreSlim _lock = new(1, 1);

    private static string FileName => AppConfig.Settings.Storage.VisitsFile;

    public static async Task RecordAsync(string guestName)
    {
        await _lock.WaitAsync();
        try
        {
            var visits = await JsonStore.LoadAsync<List<Visit>>(FileName);
            visits.Add(new Visit { Name = guestName, EnteredAt = DateTime.Now });
            await JsonStore.SaveAsync(FileName, visits);
        }
        finally
        {
            _lock.Release();
        }
    }

    public static Task<List<Visit>> GetAllAsync() => JsonStore.LoadAsync<List<Visit>>(FileName);
}
