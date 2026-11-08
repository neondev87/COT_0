// Escucha el chat y anota en visitas.json a cada invitado nuevo que entra
using COT_0.Core.Services;

namespace COT_0.HostWinUI.Services.Visitas;

/// <summary>
/// IChatService solo avisa "cambió la lista"; aquí comparas contra los Id que ya viste
/// para saber quién acaba de entrar.
/// </summary>
public sealed class VisitWatcher : IDisposable
{
    private readonly IChatService _chat;
    private readonly HashSet<string> _seen = [];
    private readonly SemaphoreSlim _lock = new(1, 1);

    public VisitWatcher(IChatService chat)
    {
        _chat = chat;
        _chat.UsersChanged += OnUsersChanged;
    }

    public void Dispose() => _chat.UsersChanged -= OnUsersChanged;

    private async void OnUsersChanged()
    {
        await _lock.WaitAsync();
        try
        {
            var users = await _chat.GetUsersAsync();
            foreach (var user in users.Where(u => _seen.Add(u.Id)))
                await VisitService.RecordAsync(user.Name);

            // Si alguien sale y vuelve a entrar, cuenta como visita nueva
            _seen.IntersectWith(users.Select(u => u.Id));
        }
        finally
        {
            _lock.Release();
        }
    }
}
