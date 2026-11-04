using COT_0.Core.Models;

namespace COT_0.Web.Services;

// Avisa cada mensaje nuevo del historial (de invitados y del sistema); misma instancia que IChatService
public interface IChatFeed
{
    event Action<ChatMessage>? MessageAdded;
}
