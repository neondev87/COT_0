using COT_0.Web.Models;

namespace COT_0.Web.Services;

// Parte el texto de un mensaje en texto, links http(s) y emotes activos de la sala
public interface IMessageFormatter
{
    FormattedMessage Format(string text);
}
