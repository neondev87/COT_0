namespace COT_0.Web.Hubs;

// Nombres de los eventos que el hub manda a los clientes
public static class ChatEvents
{
    public const string MessageReceived = "MessageReceived";
    public const string UsersChanged = "UsersChanged";
    public const string UserUpdated = "UserUpdated";
    public const string UserTyping = "UserTyping";
    public const string Kicked = "Kicked";
    public const string RoomClosed = "RoomClosed";
}
