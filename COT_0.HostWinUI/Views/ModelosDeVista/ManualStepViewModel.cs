// Un paso del manual: número, título, texto, sus notificaciones y su captura opcional (si no hay, la vista de ejemplo)
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>
/// Copia del WPF: la notificación i entra en la fase 2i+1 y sale en la 2i+2; la última se queda hasta cambiar de paso.
/// Los textos salen de Textos.xaml (Text.Manual.StepN.*) y cuántos avisos hay, de Manual:NoticeCounts.
/// La vista de ejemplo del paso la pone VistaEjemploManual según Number; Stage es la captura opcional de manual\pasos,
/// que si existe se muestra en su lugar.
/// </summary>
public partial class ManualStepViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStage), nameof(HasNoStage))]
    private ImageSource? _stage;

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
    public bool HasStage => Stage is not null;
    public bool HasNoStage => Stage is null;
    public IReadOnlyList<ManualNoticeViewModel> Notices { get; }

    /// <summary>Última fase del paso: cuando entra la última notificación.</summary>
    public int LastPhase => Notices.Count * 2 - 1;

    /// <summary>true si apareció alguna notificación nueva.</summary>
    public bool Apply(int phase) => Notices.Aggregate(false, (appeared, notice) => notice.Apply(phase) | appeared);

    public static IReadOnlyList<ManualStepViewModel> FromTexts()
    {
        var counts = AppConfig.Settings.Manual.NoticeCounts;
        var format = UiText.Get("Text.Manual.StepFormat");
        return counts.Select((count, i) =>
        {
            var key = $"Text.Manual.Step{i + 1}";
            var notices = Enumerable.Range(1, count).Select(n => UiText.Get($"{key}.Notice{n}")).ToList();
            return new ManualStepViewModel(i + 1, string.Format(format, i + 1, counts.Count),
                UiText.Find($"{key}.Title") ?? string.Empty, UiText.Get($"{key}.Text"), notices);
        }).ToList();
    }
}
