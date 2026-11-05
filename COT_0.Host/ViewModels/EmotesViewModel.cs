using System.Collections.ObjectModel;
using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.ViewModels;

// Bloque EMOTES de la sala: set de 7TV, reportes en vivo y emotes quitados
public partial class EmotesViewModel : ObservableObject
{
    private readonly IRoomEmotes _emotes;
    private readonly IConfiguration _config;

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
    private bool _hasReports;

    [ObservableProperty]
    private bool _hasRemoved;

    public EmotesViewModel(IRoomEmotes emotes, IConfiguration config)
    {
        _emotes = emotes;
        _config = config;
        _setInput = emotes.SetId ?? string.Empty;

        // Ambos eventos pueden llegar desde otro hilo (carga de 7TV, hub de los invitados)
        emotes.Changed += () => UiThread.Run(Refresh);
        emotes.Reported += _ => UiThread.Run(RefreshReports);
        Refresh();
    }

    public ObservableCollection<EmoteRowViewModel> Reports { get; } = [];
    public ObservableCollection<EmoteRowViewModel> Removed { get; } = [];

    // Campo vacío: apaga los emotes
    [RelayCommand]
    private async Task ApplyAsync()
    {
        await _emotes.UseSetAsync(string.IsNullOrWhiteSpace(SetInput) ? null : SetInput.Trim());
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
        CountText = string.Format(Text("Count"), _emotes.Active.Count);

        var removedDetail = Text("RemovedDetail");
        Removed.Clear();
        foreach (var emote in _emotes.Removed.OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase))
            Removed.Add(new EmoteRowViewModel(emote, removedDetail));
        HasRemoved = Removed.Count > 0;

        RefreshReports();
    }

    private void RefreshReports()
    {
        var format = Text("ReportCount");
        var groups = _emotes.Reports
            .Where(r => !_ignoredAt.TryGetValue(r.Emote.Id, out var at) || r.ReportedAt > at)
            .GroupBy(r => r.Emote.Id)
            .Select(g => (Emote: g.First().Emote, Count: g.Count(), Last: g.Max(r => r.ReportedAt)))
            .OrderByDescending(g => g.Count).ThenByDescending(g => g.Last);

        Reports.Clear();
        foreach (var group in groups)
            Reports.Add(new EmoteRowViewModel(group.Emote, string.Format(format, group.Count)));
        HasReports = Reports.Count > 0;
    }

    // Si falló, el motivo concreto (404, timeout, red, JSON); si no, el texto del estado
    private string StatusMessage()
    {
        var error = _emotes.Error;
        if (Status is not (EmoteSetStatus.NotFound or EmoteSetStatus.Failed) || error == EmoteSetError.None)
            return Text($"Status:{Status}");

        return string.Format(Text($"Error:{error}"), _emotes.ErrorCode);
    }

    private string Text(string key) => _config.GetText("Emotes:" + key);
}
