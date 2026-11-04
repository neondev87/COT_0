using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Models;
using COT_0.Web.Options;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

public class ProfileService(
    IChatService chat, IUserEditor editor, IRoomService room, IAvatarStore avatars, IGuestStore guests, IOptions<ChatOptions> options)
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
        if (ValidateName(name) is { } invalid) return OperationResult<User>.Fail(invalid);

        var user = await chat.AddUserAsync(new User { Name = name, PhotoUrl = form.PhotoUrl, FacultyFlag = form.FacultyFlag });
        await RememberAsync(user);
        return OperationResult<User>.Ok(user);
    }

    public async Task<OperationResult<User>> UpdateUserAsync(string userId, JoinForm form)
    {
        if (!room.IsOpen)
            return OperationResult<User>.Fail("La sala está cerrada.");

        var name = form.Name.Trim();
        if (ValidateName(name) is { } invalid) return OperationResult<User>.Fail(invalid);

        var user = await editor.UpdateUserAsync(userId, name, form.PhotoUrl, form.FacultyFlag);
        if (user is null) return OperationResult<User>.Fail("Ya no estás en la sala.");

        await RememberAsync(user);
        return OperationResult<User>.Ok(user);
    }

    public async Task<User?> ResumeAsync(string? leaveReason)
    {
        var saved = await guests.LoadAsync();
        if (saved is null) return null;

        if (leaveReason is not null || !room.IsOpen || saved.RoomCode != room.Code)
        {
            await guests.ClearAsync();
            return null;
        }

        // Si la pestaña anterior sigue viva (Safari tarda en avisar el cierre) se reusa el mismo usuario
        if (await chat.GetUserAsync(saved.UserId) is { } current) return current;

        var form = new JoinForm { Name = saved.Name, PhotoUrl = saved.PhotoUrl, FacultyFlag = saved.FacultyFlag };
        return (await CreateUserAsync(form)).Value;
    }

    private string? ValidateName(string name)
    {
        if (name.Length == 0) return "Elige un nombre.";
        if (name.Length > _options.MaxNameLength) return $"El nombre puede tener hasta {_options.MaxNameLength} caracteres.";
        return null;
    }

    private Task RememberAsync(User user) =>
        guests.SaveAsync(new StoredGuest(room.Code!, user.Id, user.Name, user.PhotoUrl, user.FacultyFlag));
}
