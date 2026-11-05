using COT_0.Web.Models;
using COT_0.Web.Options;

namespace COT_0.Web.Services;

// Recorrido del manual: bienvenida (Index -1), los pasos y el final (Index = Steps.Count)
public interface IManualTour
{
    IReadOnlyList<ManualStep> Steps { get; }
    int Index { get; }
    ManualStep? Current { get; }
    bool IsWelcome { get; }
    bool IsFinished { get; }
    bool Muted { get; }
    string NoticeAuthor { get; }
    string SoundUrl { get; }
    ManualOptions Settings { get; }
    string BootStyle { get; }
    IReadOnlyList<ManualCue> Cues { get; }
    ManualFocus Focus { get; }

    void GoTo(int index);
    void Next();
    void Previous();
    void ToggleMute();
}
