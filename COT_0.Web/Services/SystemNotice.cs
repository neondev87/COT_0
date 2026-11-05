using COT_0.Core.Models;

namespace COT_0.Web.Services;

// Avisos del sistema (entró, salió, expulsado) guardados como ChatMessage con un autor reservado y su Kind
public static class SystemNotice
{
    public const string UserId = "system";

    public static ChatMessage Create(MessageKind kind, string template, User user) =>
        new() { UserId = UserId, Kind = kind, UserName = user.Name, Text = string.Format(template, user.Name) };

    public static bool IsSystem(ChatMessage message) => message.UserId == UserId;
}
