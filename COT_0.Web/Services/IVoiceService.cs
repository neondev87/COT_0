using COT_0.Core.Models;
using COT_0.Web.Models;

namespace COT_0.Web.Services;

// Valida, guarda y publica las notas de voz en el chat
public interface IVoiceService
{
    Task<OperationResult<VoiceClip>> SendAsync(string userId, Stream audio, int durationMs);

    // null si el mensaje no es una nota de voz
    VoicePlayback? Find(ChatMessage message);
}
