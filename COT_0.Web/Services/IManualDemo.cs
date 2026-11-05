using COT_0.Core.Models;
using COT_0.Web.Models;
using COT_0.Web.Services.Emotes;

namespace COT_0.Web.Services;

// Datos de muestra para las vistas reales del manual (no se conectan a ninguna sala)
public interface IManualDemo
{
    string RoomCode { get; }
    User Me { get; }
    IReadOnlyList<User> Users { get; }
    JoinForm Profile { get; }
    MessageDraft Draft { get; }
    IReadOnlyList<MessageGroup> Groups { get; }
    RoomEmote? SampleEmote { get; }

    VoicePlayback? FindVoice(ChatMessage message);
    ImageView? FindImage(ChatMessage message);
}
