// Engancha las sesiones a la sala: abre una al abrir la sala, la cierra al cerrarla y anota a cada invitado
using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.HostWinUI.Services.Almacenamiento;
using COT_0.HostWinUI.Services.Sala;

namespace COT_0.HostWinUI.Services.Sesiones;

/// <summary>
/// Mismo patrón que VisitWatcher (sin tocar Core): escucha IChatService.UsersChanged para los invitados
/// e IRoomService.Changed para el inicio y el fin. Lo crea RoomSession al abrir la sala y lo suelta al cerrarla.
/// </summary>
public sealed class SesionWatcher : GuestWatcher
{
    private readonly IRoomService _room;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public SesionWatcher(IChatService chat, IRoomService room) : base(chat)
    {
        _room = room;
        _room.Changed += OnRoomChanged;
    }

    public override void Dispose()
    {
        _room.Changed -= OnRoomChanged;
        base.Dispose();
    }

    protected override Task OnGuestJoinedAsync(User user) => SesionService.AddGuestAsync(user.Name);

    // Abrir sala → sesión nueva; cerrar sala → hora de fin. async void: manejador de evento
    private async void OnRoomChanged()
    {
        await _lock.WaitAsync();
        try
        {
            if (_room.IsOpen && SesionService.Current is null) await SesionService.StartAsync();
            else if (!_room.IsOpen && SesionService.Current is not null) await SesionService.EndAsync();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
        }
        finally
        {
            _lock.Release();
        }
    }
}
