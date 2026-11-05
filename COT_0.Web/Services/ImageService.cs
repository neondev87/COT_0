using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Models;
using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

public sealed class ImageService(IChatService chat, IImageStore store, IOptions<ChatOptions> options) : IImageService
{
    private readonly ChatOptions _options = options.Value;

    public async Task<OperationResult<ImageClip>> SendAsync(string userId, Stream image)
    {
        if (await chat.GetUserAsync(userId) is null) return OperationResult<ImageClip>.Fail(ImageNotice.NotInRoom);

        var bytes = await ReadAsync(image);
        if (bytes is null) return OperationResult<ImageClip>.Fail(ImageNotice.ForSize(_options.MaxImageBytes));

        var info = ImageSniffer.Detect(bytes);
        if (info.Format == ImageFormat.Unknown || info.Width <= 0 || info.Height <= 0)
            return OperationResult<ImageClip>.Fail(ImageNotice.Invalid);

        var clip = new ImageClip(Guid.NewGuid().ToString("N"), bytes, info.ContentType, info.Width, info.Height);
        if (!store.TrySave(clip)) return OperationResult<ImageClip>.Fail(ImageNotice.RoomFull);

        await chat.AddMediaAsync(userId, ImageNote.ToText(clip.Id), MessageKind.Image, UrlOf(clip.Id));
        return OperationResult<ImageClip>.Ok(clip);
    }

    public ImageView? Find(ChatMessage message)
    {
        if (ImageNote.GetImageId(message.Text) is not { } id) return null;

        var url = UrlOf(id);
        if (store.Get(id) is not { } clip) return new ImageView(url, _options.ImageThumbWidth, _options.ImageThumbWidth, false);

        // Entra en ImageThumbWidth x ImageThumbMaxHeight sin deformarse
        var scale = Math.Min((double)_options.ImageThumbWidth / clip.Width, (double)_options.ImageThumbMaxHeight / clip.Height);
        scale = Math.Min(1, scale);
        return new ImageView(url, Math.Max(1, (int)Math.Round(clip.Width * scale)), Math.Max(1, (int)Math.Round(clip.Height * scale)), true);
    }

    private string UrlOf(string imageId) => $"{_options.ImagesPath.Trim('/')}/{imageId}";

    // null si pasa Chat:MaxImageBytes (el navegador ya avisa antes, esto es la validación del servidor)
    private async Task<byte[]?> ReadAsync(Stream image)
    {
        using var buffer = new MemoryStream();
        var chunk = new byte[81920];
        int read;
        while ((read = await image.ReadAsync(chunk)) > 0)
        {
            if (buffer.Length + read > _options.MaxImageBytes) return null;
            buffer.Write(chunk, 0, read);
        }
        return buffer.ToArray();
    }
}
