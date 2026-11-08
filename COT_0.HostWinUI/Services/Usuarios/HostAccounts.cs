// Lectura y escritura de usuarios.json, compartida por el login y la recuperación
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Almacenamiento;
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Validaciones;

namespace COT_0.HostWinUI.Services.Usuarios;

/// <summary>Hay una sola cuenta de host: el archivo es una lista por compatibilidad y se usa la primera.</summary>
public static class HostAccounts
{
    private static string FileName => AppConfig.Settings.Storage.UsersFile;

    public static Task<List<HostUser>> LoadAsync() => JsonStore.LoadAsync<List<HostUser>>(FileName);

    public static Task SaveAsync(List<HostUser> users) => JsonStore.SaveAsync(FileName, users);

    /// <summary>La cuenta del host, o null si todavía no se registra.</summary>
    public static HostUser? Host(List<HostUser> users) => users.FirstOrDefault();

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
