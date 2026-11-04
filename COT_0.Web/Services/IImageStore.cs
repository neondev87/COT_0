using COT_0.Web.Models;

namespace COT_0.Web.Services;

// Imágenes del chat en memoria (Singleton), con tope total de bytes por sala
public interface IImageStore
{
    // false si con esta imagen se pasa Chat:MaxRoomImageBytes
    bool TrySave(ImageClip clip);
    ImageClip? Get(string id);
    void Clear();
}
