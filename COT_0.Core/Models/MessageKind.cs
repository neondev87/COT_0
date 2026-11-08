namespace COT_0.Core.Models;

/// <summary>Tipo de mensaje del chat: de un invitado, aviso del sistema, nota de voz o imagen</summary>
public enum MessageKind
{
    Text,
    Joined,
    Left,
    Kicked,
    Voice,
    Image,
}
