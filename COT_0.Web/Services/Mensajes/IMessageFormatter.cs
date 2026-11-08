
namespace COT_0.Web.Services.Mensajes;

// Parte el texto de un mensaje en texto, links http(s) y emotes activos de la sala
public interface IMessageFormatter
{
    FormattedMessage Format(string text);
}
