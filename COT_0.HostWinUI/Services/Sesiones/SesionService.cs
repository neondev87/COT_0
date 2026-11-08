// Sesiones de invitados en sesiones.json: una por cada vez que se abre la sala (QR)
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Almacenamiento;
using COT_0.HostWinUI.Services.Configuracion;

namespace COT_0.HostWinUI.Services.Sesiones;

/// <summary>
/// Número de sesión = máximo guardado + 1, sin límite. Solo cuenta invitados: el host no tiene sesiones.
/// Hay una sola sala por proceso, así que hay a lo más una sesión abierta.
/// </summary>
public static class SesionService
{
    // La sala se abre, cierra y recibe invitados desde hilos distintos: una escritura a la vez
    private static readonly SemaphoreSlim _lock = new(1, 1);

    private static int? _current;

    private static string FileName => AppConfig.Settings.Storage.SessionsFile;

    /// <summary>Número de la sesión abierta, o null si la sala está cerrada.</summary>
    public static int? Current => _current;

    /// <summary>Abre la sesión siguiente y te regresa su número. Si ya hay una abierta, regresa esa.</summary>
    public static Task<int> StartAsync() => UpdateAsync(sessions =>
    {
        if (_current is { } open) return open;

        var number = sessions.Count == 0 ? 1 : sessions.Max(s => s.Number) + 1;
        sessions.Add(new SesionInvitados { Number = number, StartedAt = DateTime.Now });
        _current = number;
        return number;
    });

    /// <summary>Anota al invitado en la sesión abierta; sin sesión abierta no hace nada.</summary>
    public static Task AddGuestAsync(string guestName) => UpdateAsync(sessions =>
    {
        Find(sessions)?.Guests.Add(new Visit { Name = guestName, EnteredAt = DateTime.Now });
        return 0;
    });

    /// <summary>Marca la hora de fin de la sesión abierta.</summary>
    public static Task EndAsync() => UpdateAsync(sessions =>
    {
        if (Find(sessions) is { } session) session.EndedAt = DateTime.Now;
        _current = null;
        return 0;
    });

    public static Task<List<SesionInvitados>> GetAllAsync() => JsonStore.LoadAsync<List<SesionInvitados>>(FileName);

    private static SesionInvitados? Find(List<SesionInvitados> sessions) =>
        _current is { } number ? sessions.FirstOrDefault(s => s.Number == number) : null;

    // Lee, cambia y guarda dentro del candado (solo guarda si había algo que cambiar)
    private static async Task<int> UpdateAsync(Func<List<SesionInvitados>, int> change)
    {
        await _lock.WaitAsync();
        try
        {
            var sessions = await GetAllAsync();
            var wasOpen = _current;
            var result = change(sessions);
            if (wasOpen is not null || _current is not null) await JsonStore.SaveAsync(FileName, sessions);
            return result;
        }
        finally
        {
            _lock.Release();
        }
    }
}
