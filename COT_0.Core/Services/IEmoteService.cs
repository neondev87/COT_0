using COT_0.Core.Models;

namespace COT_0.Core.Services;

// Emotes de wwwroot/emotes y parseo de códigos :rana: en los mensajes
public interface IEmoteService
{
    IReadOnlyList<Emote> GetEmotes();
    IReadOnlyList<MessagePart> Parse(string text);
}
