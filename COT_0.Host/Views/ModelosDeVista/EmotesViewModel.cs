// Emotes de la sala: set de 7TV, reportes en vivo de los invitados y emotes quitados
using System.Collections.ObjectModel;
using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Tiempo;
using COT_0.Host.Services.Validaciones;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// IRoomEmotes sale del contenedor de la web (misma instancia que los chats).
/// HasReports/NoReports y el color del estado reemplazan a los DataTrigger.
/// </summary>
public partial class EmotesViewModel : ObservableObject
{
    private const int ThumbnailSize = 64;

    private readonly IRoomEmotes _emotes;

    // Reportes anteriores a IGNORAR no se cuentan; si llegan nuevos, el emote vuelve a la lista
    private readonly Dictionary<string, DateTime> _ignoredAt = [];

    [ObservableProperty]
    private string _setInput;

    [ObservableProperty]
    private EmoteSetStatus _status;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private string _countText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NoReports))]
    private bool _hasReports;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NoRemoved))]
    private bool _hasRemoved;

    public EmotesViewModel(IRoomEmotes emotes)
    {
        _emotes = emotes;
        _setInput = emotes.SetId ?? string.Empty;

        // Ambos eventos pueden llegar desde otro hilo (carga de 7TV, hub de los invitados)
        emotes.Changed += () => UiThread.Run(Refresh);
        emotes.Reported += _ => UiThread.Run(RefreshReports);
        Refresh();
    }

    public ObservableCollection<EmoteRowViewModel> Reports { get; } = [];

    public ObservableCollection<EmoteRowViewModel> Removed { get; } = [];

    public bool NoReports => !HasReports;

    public bool NoRemoved => !HasRemoved;

    // Campo vacío: apaga los emotes. Mientras 7TV responde, el estado dice CARGANDO.
    // Más largo que Inputs:MaxEmoteSetCaracteres o con espacios/símbolos (también si llegó por código) no se manda a 7TV
    [RelayCommand]
    private async Task ApplyAsync()
    {
        var input = SetInput;
        if (input.Length > InputLimits.MaxEmoteSetCharacters)
        {
            Status = EmoteSetStatus.Failed;
            StatusText = string.Format(UiText.Get("Text.Emotes.Error.TooLong"), InputLimits.MaxEmoteSetCharacters);
            return;
        }
        if (!InputRules.IsAllowed(input))
        {
            Status = EmoteSetStatus.Failed;
            StatusText = UiText.Get("Text.Emotes.Error.InvalidChars");
            return;
        }
        await _emotes.UseSetAsync(input.Length == 0 ? null : input);
        Refresh();
    }

    [RelayCommand]
    private void Remove(EmoteRowViewModel row) => _emotes.Remove(row.Id);

    [RelayCommand]
    private void Ignore(EmoteRowViewModel row)
    {
        _ignoredAt[row.Id] = DateTime.UtcNow;
        RefreshReports();
    }

    [RelayCommand]
    private void Restore(EmoteRowViewModel row) => _emotes.Restore(row.Id);

    private void Refresh()
    {
        Status = _emotes.Status;
        StatusText = StatusMessage();
        CountText = string.Format(UiText.Get("Text.Emotes.Count"), _emotes.Active.Count);

        var removedDetail = UiText.Get("Text.Emotes.RemovedDetail");
        Removed.Clear();
        foreach (var emote in _emotes.Removed.OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase))
            Removed.Add(new EmoteRowViewModel(emote, removedDetail, ThumbnailSize));
        HasRemoved = Removed.Count > 0;

        RefreshReports();
    }

    private void RefreshReports()
    {
        var format = UiText.Get("Text.Emotes.ReportCount");
        var groups = _emotes.Reports
            .Where(r => !_ignoredAt.TryGetValue(r.Emote.Id, out var at) || r.ReportedAt > at)
            .GroupBy(r => r.Emote.Id)
            .Select(g => (Emote: g.First().Emote, Count: g.Count(), Last: g.Max(r => r.ReportedAt)))
            .OrderByDescending(g => g.Count).ThenByDescending(g => g.Last);

        Reports.Clear();
        foreach (var group in groups)
            Reports.Add(new EmoteRowViewModel(group.Emote, string.Format(format, group.Count), ThumbnailSize));
        HasReports = Reports.Count > 0;
    }

    // Si falló, el motivo concreto (404, timeout, red, JSON); si no, el texto del estado
    private string StatusMessage()
    {
        var error = _emotes.Error;
        if (Status is not (EmoteSetStatus.NotFound or EmoteSetStatus.Failed) || error == EmoteSetError.None)
            return UiText.Get($"Text.Emotes.Status.{Status}");

        return string.Format(UiText.Get($"Text.Emotes.Error.{error}"), _emotes.ErrorCode);
    }
}
