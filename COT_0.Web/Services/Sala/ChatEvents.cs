namespace COT_0.Web.Services.Sala;

/// <summary>Nombres de los eventos que el hub manda a los clientes</summary>
public static class ChatEvents
{
    public const string MessageReceived = "MessageReceived";
    public const string UsersChanged = "UsersChanged";
    public const string UserUpdated = "UserUpdated";
    public const string UserTyping = "UserTyping";
    public const string Kicked = "Kicked";
    public const string RoomClosed = "RoomClosed";
}
