using COT_0.Core.Models;

namespace COT_0.Web.Services.Imagenes;

/// <summary>Valida, guarda y publica las imágenes del chat</summary>
public interface IImageService
{
    Task<OperationResult<ImageClip>> SendAsync(string userId, Stream image);

    // null si el mensaje no es una imagen
    ImageView? Find(ChatMessage message);
}
