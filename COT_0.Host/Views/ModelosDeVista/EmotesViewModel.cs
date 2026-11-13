// Emotes de la sala: cuántos compró el host (solo esos entran), reportes en vivo de los invitados y emotes quitados
using System.Collections.ObjectModel;
using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Tiempo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// IRoomEmotes sale del contenedor de la web (misma instancia que los chats). Ya no hay set de 7TV que elegir:
/// la lista la llena ChatEmoteImport con lo comprado. HasReports/NoReports y el color del estado reemplazan a los DataTrigger.
/// </summary>
public partial class EmotesViewModel : ObservableObject
{
    private const int ThumbnailSize = 64;

    private readonly IRoomEmotes _emotes;

    // Reportes anteriores a IGNORAR no se cuentan; si llegan nuevos, el emote vuelve a la lista
    private readonly Dictionary<string, DateTime> _ignoredAt = [];

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

        // Ambos eventos pueden llegar desde otro hilo (compra en vivo, hub de los invitados)
        emotes.Changed += () => UiThread.Run(Refresh);
        emotes.Reported += _ => UiThread.Run(RefreshReports);
        Refresh();
    }

    public ObservableCollection<EmoteRowViewModel> Reports { get; } = [];

    public ObservableCollection<EmoteRowViewModel> Removed { get; } = [];

    public bool NoReports => !HasReports;

    public bool NoRemoved => !HasRemoved;

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
        StatusText = UiText.Get($"Text.Emotes.Status.{Status}");
        CountText = string.Format(UiText.Get("Text.Emotes.Count"), _emotes.Active.Count + _emotes.Removed.Count);

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
}
