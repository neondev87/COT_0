using COT_0.Web.Models;

namespace COT_0.Web.Services;

// Fotos de perfil en memoria, servidas en Chat:AvatarsPath/{id}
public interface IAvatarStore
{
    string Save(byte[] bytes, string contentType);
    StoredAvatar? Get(string id);
}
