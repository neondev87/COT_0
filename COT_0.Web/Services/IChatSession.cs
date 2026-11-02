using COT_0.Core.Models;

namespace COT_0.Web.Services;

// Conexión de una pestaña al ChatHub y su estado (Scoped, uno por circuito)
public interface IChatSession : IAsyncDisposable
{
    User? CurrentUser { get; }
    IReadOnlyList<ChatMessage> Messages { get; }
    IReadOnlyList<User> Users { get; }

    event Action? Changed;

    Task<bool> StartAsync(string? userId);
    Task SendAsync(string text);
    bool IsOwn(ChatMessage message);
    Task StopAsync();
}
