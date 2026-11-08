using COT_0.Core.Models;
using COT_0.Core.Services;

namespace COT_0.Web.Services.Emotes;

/// <summary>Implementa IEmoteActions: menú de un emote del chat (Scoped); reportar le llega al host por IRoomEmotes.Reported.</summary>
public sealed class EmoteActions(IRoomEmotes emotes) : IEmoteActions
{
    public RoomEmote? Target { get; private set; }
    public bool? LastReportSent { get; private set; }
    public int NoticeKey { get; private set; }

    public event Action? Changed;

    public void Open(RoomEmote emote)
    {
        Target = emote;
        LastReportSent = null;
        Changed?.Invoke();
    }

    public void Close()
    {
        Target = null;
        Changed?.Invoke();
    }

    // El reporte le llega al host por IRoomEmotes.Reported
    public void Report(User reporter)
    {
        if (Target is not { } emote) return;

        LastReportSent = emotes.Report(emote.Id, reporter.Id, reporter.Name);
        NoticeKey++;
        Target = null;
        Changed?.Invoke();
    }
}
