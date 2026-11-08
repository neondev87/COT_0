// Lectura y escritura de usuarios.json (hasta Usuarios:Maximo), compartida por el login, el registro y la recuperación
using System.Text.Json;
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Validaciones;

namespace COT_0.Host.Services.Usuarios;

/// <summary>Lista de usuarios; si el archivo trae una cuenta suelta (formato de una sola cuenta) se lee como el primero.</summary>
public static class HostAccounts
{
    private static string FileName => AppConfig.Settings.Storage.UsersFile;

    public static int Maximum => AppConfig.Settings.Usuarios.Maximo;

    /// <summary>Lee la lista tal cual: no valida ni recorta nombres, así no se pierde ninguna cuenta guardada.</summary>
    public static async Task<List<HostUser>> LoadAsync()
    {
        var root = await JsonStore.LoadAsync<JsonElement>(FileName);
        return root.ValueKind switch
        {
            JsonValueKind.Array => root.Deserialize<List<HostUser>>() ?? [],
            JsonValueKind.Object => root.Deserialize<HostUser>() is { } single ? [single] : [],
            _ => [],
        };
    }

    public static Task SaveAsync(List<HostUser> users) => JsonStore.SaveAsync(FileName, users);

    /// <summary>Nombres sin distinguir mayúsculas y minúsculas.</summary>
    public static HostUser? Find(List<HostUser> users, string userName) =>
        users.FirstOrDefault(u => string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase));

    public static void SetPassword(HostUser user, string password)
    {
        var hashed = SecretHasher.Create(password);
        user.PasswordHash = hashed.Hash;
        user.Salt = hashed.Salt;
    }

    /// <summary>Guarda la pregunta y la respuesta ya normalizada y hasheada.</summary>
    public static void SetSecurityQuestion(HostUser user, string questionKey, string answer)
    {
        var hashed = SecretHasher.Create(SecurityAnswerValidator.Normalize(answer));
        user.SecurityQuestion = questionKey;
        user.AnswerHash = hashed.Hash;
        user.AnswerSalt = hashed.Salt;
    }

    public static bool HasSecurityQuestion(HostUser user) =>
        !string.IsNullOrEmpty(user.SecurityQuestion) && !string.IsNullOrEmpty(user.AnswerHash);
}
