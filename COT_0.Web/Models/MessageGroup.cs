using COT_0.Core.Models;

namespace COT_0.Web.Models;

// Mensajes seguidos del mismo autor (o un aviso del sistema solo); Key = Id del primero
public sealed record MessageGroup(bool IsSystem, bool IsOwn, IReadOnlyList<ChatMessage> Messages)
{
    public ChatMessage First => Messages[0];
    public string Key => First.Id;

    // Mismos mensajes (las mismas instancias: al editar un perfil llegan copias nuevas) y mismo tipo de grupo
    public bool SameAs(MessageGroup other) =>
        IsSystem == other.IsSystem && IsOwn == other.IsOwn
        && Messages.Count == other.Messages.Count
        && Messages.Select((message, i) => ReferenceEquals(message, other.Messages[i])).All(same => same);
}
