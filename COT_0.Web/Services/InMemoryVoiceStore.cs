using System.Collections.Concurrent;
using COT_0.Web.Models;
using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

public sealed class InMemoryVoiceStore(IOptions<VoiceOptions> options) : IVoiceStore
{
    private readonly ConcurrentDictionary<string, VoiceClip> _clips = new();
    private readonly ConcurrentQueue<string> _order = new();

    public void Save(VoiceClip clip)
    {
        _clips[clip.Id] = clip;
        _order.Enqueue(clip.Id);

        // Pasado el tope se van los más viejos
        while (_clips.Count > options.Value.MaxClips && _order.TryDequeue(out var oldest)) _clips.TryRemove(oldest, out _);
    }

    public VoiceClip? Get(string id) => _clips.TryGetValue(id, out var clip) ? clip : null;

    public void Clear()
    {
        _clips.Clear();
        _order.Clear();
    }
}
