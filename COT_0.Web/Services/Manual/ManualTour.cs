using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Manual;

/// <summary>Implementa IManualTour: recorrido del manual (bienvenida, pasos y final).</summary>
public sealed class ManualTour(IOptions<ManualOptions> options, IManualDemo demo, IEmoteActions emoteActions) : IManualTour
{
    private readonly ManualOptions _options = options.Value;

    public IReadOnlyList<ManualStep> Steps => _options.Steps;
    public int Index { get; private set; } = -1;
    public ManualStep? Current => IsWelcome || IsFinished ? null : Steps[Index];
    public bool IsWelcome => Index < 0;
    public bool IsFinished => Index >= Steps.Count;
    public bool Muted { get; private set; }
    public string NoticeAuthor => _options.NoticeAuthor;
    public string SoundUrl => _options.SoundUrl;
    public ManualOptions Settings => _options;

    // Duración del arranque como variable CSS (--manual-boot)
    public string BootStyle => $"--manual-boot:{ManualTimeline.Seconds(_options.BootSeconds)}";

    public IReadOnlyList<ManualCue> Cues => ManualTimeline.Cues(NoticeTexts, _options);
    public ManualFocus Focus => ManualTimeline.Focus(Current?.Notices ?? [], _options);

    private IReadOnlyList<string> NoticeTexts =>
        Current?.Notices.Select(n => n.Text).ToList() ?? [IsWelcome ? _options.WelcomeNotice : _options.FinishNotice];

    public void GoTo(int index)
    {
        Index = Math.Clamp(index, -1, Steps.Count);
        SyncEmoteMenu();
    }

    public void Next() => GoTo(Index + 1);

    public void Previous() => GoTo(Math.Max(Index - 1, 0));

    public void ToggleMute() => Muted = !Muted;

    // En el paso de emotes se abre el menú de reportar con un emote de muestra
    private void SyncEmoteMenu()
    {
        if (Current?.View == ManualView.Emotes && demo.SampleEmote is { } emote) emoteActions.Open(emote);
        else emoteActions.Close();
    }
}
