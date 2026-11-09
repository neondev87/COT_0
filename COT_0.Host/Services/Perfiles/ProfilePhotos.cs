// Fotos de perfil de los usuarios en %LocalAppData%\COT_0\perfiles (fuera del repo): guardar, leer, mover y borrar
using System.Runtime.InteropServices;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Configuracion;
using Windows.Storage;

namespace COT_0.Host.Services.Perfiles;

/// <summary>
/// Un PNG cuadrado por usuario (nombre en minúsculas). Sin archivo, la vista pinta la inicial con color.
/// Changed avisa qué usuario cambió de foto para que la firma y las tarjetas se repinten.
/// </summary>
public static class ProfilePhotos
{
    public static event Action<string>? Changed;

    private static string Folder => Path.Combine(JsonStore.Folder, AppConfig.Settings.Storage.ProfilesFolder);

    public static string PathFor(string userName) =>
        Path.Combine(Folder, string.Format(AppConfig.Settings.Profiles.PhotoFileFormat, userName.ToLowerInvariant()));

    /// <summary>Recorta, reduce y guarda la imagen elegida; reemplaza la anterior.</summary>
    public static async Task SaveAsync(string userName, StorageFile image)
    {
        byte[] png;
        using (var source = await image.OpenReadAsync())
            png = await PhotoCropper.CropAsync(source);

        var folder = Folder;
        await Task.Run(() => Directory.CreateDirectory(folder));
        await using (var file = AsyncFiles.Create(PathFor(userName)))
            await file.WriteAsync(png);
        Changed?.Invoke(userName);
    }

    /// <summary>Bytes del PNG, o null si ese usuario no tiene foto.</summary>
    public static async Task<byte[]?> LoadAsync(string userName)
    {
        var path = PathFor(userName);
        if (!File.Exists(path)) return null;

        await using var file = AsyncFiles.OpenRead(path);
        var bytes = new byte[file.Length];
        await file.ReadExactlyAsync(bytes);
        return bytes;
    }

    public static async Task DeleteAsync(string userName)
    {
        var path = PathFor(userName);
        await Task.Run(() => File.Delete(path));
        Changed?.Invoke(userName);
    }

    /// <summary>CAMBIAR NOMBRE: la foto se va con la cuenta.</summary>
    public static async Task MoveAsync(string oldName, string newName)
    {
        var (from, to) = (PathFor(oldName), PathFor(newName));
        if (from == to || !File.Exists(from)) return;
        await Task.Run(() => File.Move(from, to, overwrite: true));
        Changed?.Invoke(newName);
    }

    /// <summary>true si el error viene de la imagen (formato que Windows no decodifica) o del disco.</summary>
    public static bool IsPhotoError(Exception ex) =>
        JsonStore.IsStorageError(ex) || ex is COMException or ArgumentException;
}
