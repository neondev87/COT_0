using COT_0.Core.Models;
using COT_0.Web.Models;

namespace COT_0.Web.Services;

// Arma los grupos del chat a partir de la lista de mensajes (del más viejo al más nuevo)
public static class MessageGrouping
{
    public static IReadOnlyList<MessageGroup> Group(IReadOnlyList<ChatMessage> messages, string? ownUserId, TimeSpan window)
    {
        var groups = new List<MessageGroup>();
        var current = new List<ChatMessage>();

        foreach (var message in messages)
        {
            if (current.Count > 0 && !Continues(current[^1], message, window))
            {
                groups.Add(Close(current, ownUserId));
                current = [];
            }

            current.Add(message);
        }

        if (current.Count > 0) groups.Add(Close(current, ownUserId));
        return groups;
    }

    private static bool Continues(ChatMessage previous, ChatMessage next, TimeSpan window) =>
        !SystemNotice.IsSystem(previous) && !SystemNotice.IsSystem(next)
        && previous.UserId == next.UserId
        && next.SentAt - previous.SentAt <= window;

    private static MessageGroup Close(List<ChatMessage> messages, string? ownUserId) =>
        new(SystemNotice.IsSystem(messages[0]), messages[0].UserId == ownUserId, messages);
}
