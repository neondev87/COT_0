namespace COT_0.Web.Services.Perfil;

/// <summary>Fotos de perfil en memoria, servidas en Chat:AvatarsPath/{id}</summary>
public interface IAvatarStore
{
    string Save(byte[] bytes, string contentType);
    StoredAvatar? Get(string id);
}
