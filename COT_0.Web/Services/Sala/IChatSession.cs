using COT_0.Core.Models;

namespace COT_0.Web.Services.Sala;

// Conexión de una pestaña al ChatHub y su estado (Scoped, uno por circuito)
public interface IChatSession : IAsyncDisposable
{
    User? CurrentUser { get; }
    IReadOnlyList<MessageGroup> Groups { get; }
    IReadOnlyList<User> Users { get; }

    // "Fulano está escribiendo…" o null
    string? TypingSummary { get; }

    event Action? Changed;

    // El invitado tiene que salir del chat; el string es un LeaveReason (null = volver a entrar solo)
    event Action<string?>? Ended;

    Task<bool> StartAsync(string? userId);
    Task SendAsync(string text);
    Task NotifyTypingAsync();
    Task StopAsync();
}
