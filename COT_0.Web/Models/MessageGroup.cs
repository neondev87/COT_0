using COT_0.Core.Models;

namespace COT_0.Web.Models;

// Mensajes seguidos del mismo autor (o un aviso del sistema solo); Key = Id del primero
public sealed record MessageGroup(bool IsSystem, bool IsOwn, IReadOnlyList<ChatMessage> Messages)
{
    public ChatMessage First => Messages[0];
    public string Key => First.Id;
}
