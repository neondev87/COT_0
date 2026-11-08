using System.Collections.Concurrent;

namespace COT_0.Web.Services.Perfil;

/// <summary>Implementa IAvatarStore: fotos de perfil de los invitados en memoria.</summary>
public class InMemoryAvatarStore : IAvatarStore
{
    private readonly ConcurrentDictionary<string, StoredAvatar> _avatars = new();

    public string Save(byte[] bytes, string contentType)
    {
        var id = Guid.NewGuid().ToString("N");
        _avatars[id] = new StoredAvatar(bytes, contentType);
        return id;
    }

    public StoredAvatar? Get(string id) => _avatars.TryGetValue(id, out var avatar) ? avatar : null;
}
