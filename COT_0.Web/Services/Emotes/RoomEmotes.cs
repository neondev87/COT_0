using COT_0.Core.Models;
using COT_0.Core.Services;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Emotes;

// Cachea en memoria la lista del set (Chat:EmoteCacheMinutes); si 7TV falla se queda con la última que tenía
public sealed class RoomEmotes(ISevenTvClient client, IOptions<ChatOptions> options) : IRoomEmotes
{
    private readonly object _gate = new();
    private readonly HashSet<string> _removedIds = [];
    private readonly List<EmoteReport> _reports = [];
    private IReadOnlyList<RoomEmote> _all = [];
    private EmoteList _list = EmoteList.Empty;
    private string? _setId = EmoteSetId.Parse(options.Value.EmoteSetId);
    private DateTime _expiresAt = DateTime.MinValue;
    private Task<EmoteSetStatus>? _loading;
    private string? _loadingSetId;
    private EmoteSetStatus _status;

    public event Action? Changed;
    public event Action<EmoteReport>? Reported;

    public string? SetId => _setId;
    public string? SetName { get; private set; }
    public int Version { get; private set; }
    public EmoteSetError Error { get; private set; }
    public int? ErrorCode { get; private set; }

    // Sin set: Off, salvo que el host haya pasado un ID inválido (NotFound, para mostrar el aviso)
    public EmoteSetStatus Status => Fresh(() =>
        _setId is null ? (_status == EmoteSetStatus.NotFound ? _status : EmoteSetStatus.Off)
        : _loading is not null && _all.Count == 0 ? EmoteSetStatus.Loading
        : _status);
    public IReadOnlyList<RoomEmote> Active => Fresh(() => _list.Active);
    public IReadOnlyList<RoomEmote> Removed => Fresh(() => _list.Removed);
    public IReadOnlyList<EmoteReport> Reports { get { lock (_gate) return _reports.ToList(); } }

    public async Task<EmoteSetStatus> UseSetAsync(string? setIdOrUrl)
    {
        Task<EmoteSetStatus>? loading;
        lock (_gate)
        {
            _setId = EmoteSetId.Parse(setIdOrUrl);
            var invalid = _setId is null && !string.IsNullOrWhiteSpace(setIdOrUrl);
            _status = invalid ? EmoteSetStatus.NotFound : EmoteSetStatus.Loading;
            Error = invalid ? EmoteSetError.InvalidId : EmoteSetError.None;
            ErrorCode = null;
            _all = [];
            SetName = null;
            _expiresAt = DateTime.MinValue;
            Rebuild();
            loading = StartLoad();
        }

        Changed?.Invoke();
        return loading is null ? Status : await loading;
    }

    public RoomEmote? Find(string name) => Fresh(() => _list.ByName.GetValueOrDefault(name));

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

    // Al quitarlo se resuelven sus reportes
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
            Rebuild();
        }

        Changed?.Invoke();
    }

    // Lee el estado y, si la cache venció, pide la lista de nuevo en segundo plano
    private T Fresh<T>(Func<T> read)
    {
        lock (_gate)
        {
            if (DateTime.UtcNow >= _expiresAt) StartLoad();
            return read();
        }
    }

    private Task<EmoteSetStatus>? StartLoad()
    {
        if (_setId is null) return null;
        if (_loading is not null && _loadingSetId == _setId) return _loading;

        // Una carga de otro set (ej. el de arranque) no bloquea al set nuevo
        var setId = _setId;
        _loadingSetId = setId;
        return _loading = Task.Run(() => LoadAsync(setId));
    }

    private async Task<EmoteSetStatus> LoadAsync(string setId)
    {
        var result = await client.GetEmoteSetAsync(setId);
        var o = options.Value;

        lock (_gate)
        {
            if (_loadingSetId == setId) _loading = null;
            if (setId != _setId) return result.Status;

            var ok = result.Status == EmoteSetStatus.Ready;
            _expiresAt = DateTime.UtcNow + (ok ? TimeSpan.FromMinutes(o.EmoteCacheMinutes) : TimeSpan.FromSeconds(o.EmoteRetrySeconds));
            if (ok)
            {
                _all = result.Emotes;
                SetName = result.Name;
            }

            // Si falla un refresco se siguen usando los emotes que ya había
            _status = ok || _all.Count == 0 ? result.Status : EmoteSetStatus.Ready;
            Error = _status == EmoteSetStatus.Ready ? EmoteSetError.None : result.Error;
            ErrorCode = _status == EmoteSetStatus.Ready ? null : result.HttpCode;
            Rebuild();
        }

        Changed?.Invoke();
        return result.Status;
    }

    private void Rebuild()
    {
        _list = EmoteList.From(_all, _removedIds);
        Version++;
    }
}
