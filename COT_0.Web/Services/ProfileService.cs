using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Models;
using COT_0.Web.Options;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

public class ProfileService(IChatService chat, IRoomService room, IAvatarStore avatars, IOptions<ChatOptions> options)
    : IProfileService
{
    private readonly ChatOptions _options = options.Value;

    public async Task<OperationResult<string>> SavePhotoAsync(IBrowserFile file)
    {
        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return OperationResult<string>.Fail("El archivo tiene que ser una imagen.");

        // El navegador la achica antes de subirla
        var resized = await file.RequestImageFileAsync(_options.PhotoFormat, _options.PhotoSize, _options.PhotoSize);
        if (resized.Size > _options.MaxPhotoBytes)
            return OperationResult<string>.Fail("La foto es demasiado grande.");

        using var buffer = new MemoryStream();
        await using (var input = resized.OpenReadStream(_options.MaxPhotoBytes))
        {
            await input.CopyToAsync(buffer);
        }

        var id = avatars.Save(buffer.ToArray(), resized.ContentType);
        return OperationResult<string>.Ok($"{_options.AvatarsPath.Trim('/')}/{id}");
    }

    public async Task<OperationResult<User>> CreateUserAsync(JoinForm form)
    {
        if (!room.IsOpen)
            return OperationResult<User>.Fail("La sala está cerrada.");

        var name = form.Name.Trim();
        if (name.Length == 0)
            return OperationResult<User>.Fail("Elegí un nombre.");
        if (name.Length > _options.MaxNameLength)
            return OperationResult<User>.Fail($"El nombre puede tener hasta {_options.MaxNameLength} caracteres.");

        var user = new User { Name = name, PhotoUrl = form.PhotoUrl, FacultyFlag = form.FacultyFlag };
        return OperationResult<User>.Ok(await chat.AddUserAsync(user));
    }
}
