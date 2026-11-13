using COT_0.Core.Models;
using COT_0.Core.Services;

namespace COT_0.Web.Services.Emotes;

/// <summary>
/// Emotes de la sala: SOLO los que compró en su tienda el host que la abrió (los manda el Host con SetPurchased).
/// No se pide ningún set a 7TV: sin compras el estado es Off y el chat no muestra emotes.
/// Si dos comprados se llaman igual, gana el primero (los nombres de 7TV distinguen mayúsculas)
/// </summary>
public sealed class RoomEmotes : IRoomEmotes, IPurchasedEmotes
{
    private readonly object _gate = new();
    private readonly HashSet<string> _removedIds = [];
    private readonly List<EmoteReport> _reports = [];
    private IReadOnlyList<RoomEmote> _purchased = [];
    private EmoteList _list = EmoteList.Empty;

    public event Action? Changed;
    public event Action<EmoteReport>? Reported;

    // Del contrato de Core (de cuando había set de 7TV); ya no hay set: siempre vacíos (ver CONTRATO.md)
    public string? SetId => null;
    public string? SetName => null;
    public EmoteSetError Error => EmoteSetError.None;
    public int? ErrorCode => null;

    public int Version { get; private set; }
    public EmoteSetStatus Status { get { lock (_gate) return _purchased.Count > 0 ? EmoteSetStatus.Ready : EmoteSetStatus.Off; } }
    public IReadOnlyList<RoomEmote> Active { get { lock (_gate) return _list.Active; } }
    public IReadOnlyList<RoomEmote> Removed { get { lock (_gate) return _list.Removed; } }
    public IReadOnlyList<EmoteReport> Reports { get { lock (_gate) return _reports.ToList(); } }
    public IReadOnlyList<RoomEmote> Purchased { get { lock (_gate) return _purchased; } }

    public void SetPurchased(IReadOnlyList<RoomEmote> emotes) => Update(() =>
    {
        _purchased = emotes.DistinctBy(e => e.Name, StringComparer.Ordinal).ToList();
        _reports.RemoveAll(r => _purchased.All(e => e.Id != r.Emote.Id));
        return true;
    });

    // La fuente de emotes es solo lo comprado: cualquier set que llegue se ignora
    public Task<EmoteSetStatus> UseSetAsync(string? setIdOrUrl) => Task.FromResult(Status);

    public RoomEmote? Find(string name)
    {
        lock (_gate) return _list.ByName.GetValueOrDefault(name);
    }

    public IReadOnlyList<RoomEmote> Search(string? query, int max) => EmoteSearch.Find(Active, query, max);

    public bool Report(string emoteId, string userId, string userName)
    {
        EmoteReport report;
        lock (_gate)
        {
            var emote = _list.Active.FirstOrDefault(e => e.Id == emoteId);
            if (emote is null || _reports.Any(r => r.Emote.Id == emoteId && r.UserId == userId)) return false;

            report = new EmoteReport(emote, userId, userName, DateTime.UtcNow);
            _reports.Add(report);
        }

        Reported?.Invoke(report);
        return true;
    }

    // Al quitarlo se resuelven sus reportes (QUITAR solo lo esconde de la sala: la compra sigue siendo del host)
    public void Remove(string emoteId) => Update(() =>
    {
        _reports.RemoveAll(r => r.Emote.Id == emoteId);
        return _removedIds.Add(emoteId);
    });

    public void Restore(string emoteId) => Update(() => _removedIds.Remove(emoteId));

    public void ClearReports()
    {
        lock (_gate) _reports.Clear();
    }

    private void Update(Func<bool> change)
    {
        lock (_gate)
        {
            if (!change()) return;
            _list = EmoteList.From(_purchased, _removedIds);
            Version++;
        }

        Changed?.Invoke();
    }
}
