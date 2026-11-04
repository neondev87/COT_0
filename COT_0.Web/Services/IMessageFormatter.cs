using COT_0.Web.Models;

namespace COT_0.Web.Services;

// Parte el texto de un mensaje en texto, links http(s) y emotes
public interface IMessageFormatter
{
    IReadOnlyList<MessageToken> Format(string text);
}
