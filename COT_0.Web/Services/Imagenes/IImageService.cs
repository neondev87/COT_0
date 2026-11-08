using COT_0.Core.Models;

namespace COT_0.Web.Services.Imagenes;

// Valida, guarda y publica las imágenes del chat
public interface IImageService
{
    Task<OperationResult<ImageClip>> SendAsync(string userId, Stream image);

    // null si el mensaje no es una imagen
    ImageView? Find(ChatMessage message);
}
