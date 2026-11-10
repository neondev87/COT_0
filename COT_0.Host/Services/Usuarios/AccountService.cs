// Cuentas desde USUARIOS (04): eliminar (con sus compras, tickets y foto), cambiar nombre y cambiar contraseña
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Perfiles;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Validaciones;

namespace COT_0.Host.Services.Usuarios;

/// <summary>
/// Vuelve a validar todo antes de guardar. Solo un ADMIN elimina cuentas (sin respuesta secreta); nombre y contraseña
/// solo los cambia su dueño y primero pide su respuesta secreta (se compara contra su hash; si falla no cambia nada).
/// </summary>
public class AccountService(PurchaseService purchases)
{
    public static bool CanManage(string userName) => ActiveUser.Is(userName);

    public static bool CanDelete(string userName) => ActiveUser.IsAdmin;

    public async Task<CredentialError> DeleteAsync(string userName)
    {
        if (!CanDelete(userName)) return CredentialError.NotAllowed;

        var users = await HostAccounts.LoadAsync();
        if (HostAccounts.Find(users, userName) is not { } user) return CredentialError.WrongCredentials;

        users.Remove(user);
        await HostAccounts.SaveAsync(users);
        await purchases.RemoveUserAsync(user.UserName);
        await ProfilePhotos.DeleteAsync(user.UserName);
        return CredentialError.None;
    }

    /// <summary>Mismas reglas que el registro y sin repetir nombre; compras, tickets y foto se van con la cuenta.</summary>
    public async Task<CredentialError> RenameAsync(string userName, string answer, string newName)
    {
        if (!CanManage(userName)) return CredentialError.NotAllowed;
        var error = CredentialValidator.ValidateUserName(newName);
        if (error != CredentialError.None) return error;

        var users = await HostAccounts.LoadAsync();
        var (user, check) = Verify(users, userName, answer);
        if (check != CredentialError.None) return check;
        var other = HostAccounts.Find(users, newName);
        if (other is not null && other != user) return CredentialError.UserTaken;

        var oldName = user!.UserName;
        user.UserName = newName;
        await HostAccounts.SaveAsync(users);
        try
        {
            await purchases.RenameUserAsync(oldName, newName);
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            // compras.json no se pudo guardar: la cuenta regresa a su nombre para no separar sus compras
            user.UserName = oldName;
            await HostAccounts.SaveAsync(users);
            return CredentialError.RenameFailed;
        }
        await ProfilePhotos.MoveAsync(oldName, newName);
        ActiveUser.Rename(newName);
        return CredentialError.None;
    }

    /// <summary>Pide la respuesta secreta; la nueva contraseña cumple las reglas vigentes y se escribe igual dos veces.</summary>
    public async Task<CredentialError> ChangePasswordAsync(string userName, string answer, string password, string confirmation)
    {
        if (!CanManage(userName)) return CredentialError.NotAllowed;
        var error = CredentialValidator.ValidateNewPassword(password, confirmation);
        if (error != CredentialError.None) return error;

        var users = await HostAccounts.LoadAsync();
        var (user, check) = Verify(users, userName, answer);
        if (check != CredentialError.None) return check;

        HostAccounts.SetPassword(user!, password);
        await HostAccounts.SaveAsync(users);
        return CredentialError.None;
    }

    // Sin pregunta configurada pide configurarla; si la respuesta no coincide, un mensaje genérico
    private static (HostUser? User, CredentialError Error) Verify(List<HostUser> users, string userName, string answer)
    {
        if (HostAccounts.Find(users, userName) is not { } user) return (null, CredentialError.WrongAnswer);
        if (!HostAccounts.HasSecurityQuestion(user)) return (null, CredentialError.NoSecurityQuestion);
        if (string.IsNullOrEmpty(answer)) return (null, CredentialError.AnswerEmpty);
        var matches = SecurityAnswerValidator.ValidateAnswer(answer) == CredentialError.None
            && SecretHasher.Verify(SecurityAnswerValidator.Normalize(answer), user.AnswerHash, user.AnswerSalt);
        return matches ? (user, CredentialError.None) : (null, CredentialError.WrongAnswer);
    }
}
