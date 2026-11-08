using COT_0.Core.Models;

namespace COT_0.Web.Services.Sala;

/// <summary>Avisa cada mensaje nuevo del historial (de invitados y del sistema); misma instancia que IChatService</summary>
public interface IChatFeed
{
    event Action<ChatMessage>? MessageAdded;
}
