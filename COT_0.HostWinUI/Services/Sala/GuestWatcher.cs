// Base de los vigilantes del chat: detecta qué invitado acaba de entrar sin tocar Core
using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.HostWinUI.Services.Almacenamiento;

namespace COT_0.HostWinUI.Services.Sala;

/// <summary>
/// IChatService solo avisa "cambió la lista"; aquí se compara contra los Id ya vistos para saber
/// quién entró. La usan VisitWatcher (visitas.json) y SesionWatcher (sesiones.json).
/// </summary>
public abstract class GuestWatcher : IDisposable
{
    private readonly HashSet<string> _seen = [];
    private readonly SemaphoreSlim _lock = new(1, 1);

    protected GuestWatcher(IChatService chat)
    {
        Chat = chat;
        Chat.UsersChanged += OnUsersChanged;
    }

    protected IChatService Chat { get; }

    public virtual void Dispose() => Chat.UsersChanged -= OnUsersChanged;

    /// <summary>Se llama una vez por cada invitado nuevo, en orden y de uno en uno.</summary>
    protected abstract Task OnGuestJoinedAsync(User user);

    // async void: es un manejador de evento; los errores de disco no deben tumbar la app
    private async void OnUsersChanged()
    {
        await _lock.WaitAsync();
        try
        {
            var users = await Chat.GetUsersAsync();
            foreach (var user in users.Where(u => _seen.Add(u.Id)))
                await OnGuestJoinedAsync(user);

            // Si alguien sale y vuelve a entrar, cuenta como entrada nueva
            _seen.IntersectWith(users.Select(u => u.Id));
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
