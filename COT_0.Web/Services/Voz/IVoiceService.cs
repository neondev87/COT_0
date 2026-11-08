using COT_0.Core.Models;

namespace COT_0.Web.Services.Voz;

/// <summary>Valida, guarda y publica las notas de voz en el chat</summary>
public interface IVoiceService
{
    Task<OperationResult<VoiceClip>> SendAsync(string userId, Stream audio, int durationMs);

    // null si el mensaje no es una nota de voz
    VoicePlayback? Find(ChatMessage message);
}
