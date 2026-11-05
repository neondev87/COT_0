using COT_0.Core.Models;

namespace COT_0.Web.Services;

// Copia los datos visibles del autor (nombre, foto, escudo) en un mensaje
public static class MessageAuthor
{
    public static void Apply(ChatMessage message, User author)
    {
        message.UserName = author.Name;
        message.UserPhotoUrl = author.PhotoUrl;
        message.UserFacultyFlag = author.FacultyFlag;
    }

    // Mensaje nuevo con el mismo Id (Blazor conserva el elemento por @key)
    public static ChatMessage With(ChatMessage message, User author)
    {
        var copy = new ChatMessage
        {
            Id = message.Id,
            UserId = message.UserId,
            Kind = message.Kind,
            Text = message.Text,
            AudioUrl = message.AudioUrl,
            ImageUrl = message.ImageUrl,
            SentAt = message.SentAt,
        };
        Apply(copy, author);
        return copy;
    }
}
