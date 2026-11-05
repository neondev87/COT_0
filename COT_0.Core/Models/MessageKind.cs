namespace COT_0.Core.Models;

// Tipo de mensaje del chat: de un invitado, aviso del sistema, nota de voz o imagen
public enum MessageKind
{
    Text,
    Joined,
    Left,
    Kicked,
    Voice,
    Image,
}
