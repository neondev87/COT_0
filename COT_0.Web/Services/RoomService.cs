using System.Security.Cryptography;
using COT_0.Core.Services;
using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

// Estado de la sala en memoria (Singleton)
public class RoomService(IChatService chat, IVoiceStore voices, IImageStore images, IOptions<ChatOptions> options) : IRoomService
{
    public event Action? Changed;

    public string? Code { get; private set; }
    public DateTime? OpenedAt { get; private set; }
    public bool IsOpen => Code is not null;

    public void Open()
    {
        var o = options.Value;
        Code = RandomNumberGenerator.GetString(o.RoomCodeChars, o.RoomCodeLength);
        OpenedAt = DateTime.UtcNow;
        Changed?.Invoke();
    }

    public async Task CloseAsync()
    {
        Code = null;
        OpenedAt = null;
        await chat.ClearAsync();
        voices.Clear();
        images.Clear();
        Changed?.Invoke();
    }
}
