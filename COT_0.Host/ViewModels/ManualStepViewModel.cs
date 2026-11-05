namespace COT_0.Host.ViewModels;

// Un paso del manual: número, título, texto y sus notificaciones.
// La notificación i entra en la fase 2i+1 y sale en la 2i+2; la última se queda hasta cambiar de paso
public sealed class ManualStepViewModel
{
    public ManualStepViewModel(int number, string label, string title, string text, IReadOnlyList<string> notices)
    {
        Number = number;
        Label = label;
        Title = title;
        Text = text;
        Notices = notices
            .Select((notice, i) => new ManualNoticeViewModel(notice, 2 * i + 1, i == notices.Count - 1 ? int.MaxValue : 2 * i + 1))
            .ToList();
    }

    public int Number { get; }
    public string Label { get; }
    public string Title { get; }
    public string Text { get; }
    public bool HasTitle => Title.Length > 0;
    public IReadOnlyList<ManualNoticeViewModel> Notices { get; }

    // Última fase del paso: cuando entra la última notificación
    public int LastPhase => Notices.Count * 2 - 1;

    // true si apareció alguna notificación nueva
    public bool Apply(int phase) => Notices.Aggregate(false, (appeared, notice) => notice.Apply(phase) | appeared);
}
