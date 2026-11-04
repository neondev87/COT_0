namespace COT_0.Web.Models;

// Quiénes están escribiendo; cada aviso vence solo después de "timeout"
public sealed class TypingTracker(TimeSpan timeout) : IDisposable
{
    private readonly Dictionary<string, (string Name, DateTime Until)> _typing = [];
    private readonly object _lock = new();
    private Timer? _timer;

    public event Action? Changed;

    public string? Summary
    {
        get
        {
            string[] names;
            lock (_lock) names = [.. _typing.Values.Select(t => t.Name)];

            return names.Length switch
            {
                0 => null,
                1 => $"{names[0]} está escribiendo…",
                2 => $"{names[0]} y {names[1]} están escribiendo…",
                _ => "Varios están escribiendo…",
            };
        }
    }

    public void Mark(string userId, string name)
    {
        lock (_lock)
        {
            _typing[userId] = (name, DateTime.UtcNow + timeout);
            _timer ??= new Timer(_ => Expire(), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        Changed?.Invoke();
    }

    public void Clear(string userId)
    {
        bool removed;
        lock (_lock) removed = _typing.Remove(userId);
        if (removed) Changed?.Invoke();
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _timer?.Dispose();
            _timer = null;
        }
    }

    private void Expire()
    {
        int removed;
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            removed = _typing.Where(t => t.Value.Until <= now).Select(t => t.Key).ToList().Count(_typing.Remove);

            // Sin nadie escribiendo el timer se apaga hasta el próximo aviso
            if (_typing.Count == 0) Dispose();
        }

        if (removed > 0) Changed?.Invoke();
    }
}
