using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Imagenes;

public sealed class InMemoryImageStore(IOptions<ChatOptions> options) : IImageStore
{
    private readonly ConcurrentDictionary<string, ImageClip> _images = new();
    private readonly object _gate = new();
    private long _totalBytes;

    public bool TrySave(ImageClip clip)
    {
        lock (_gate)
        {
            if (_totalBytes + clip.Bytes.Length > options.Value.MaxRoomImageBytes) return false;
            _totalBytes += clip.Bytes.Length;
            _images[clip.Id] = clip;
            return true;
        }
    }

    public ImageClip? Get(string id) => _images.TryGetValue(id, out var clip) ? clip : null;

    public void Clear()
    {
        lock (_gate)
        {
            _images.Clear();
            _totalBytes = 0;
        }
    }
}
