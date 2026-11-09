// Cuentas desde USUARIOS (04): eliminar (con sus compras, tickets y foto), cambiar nombre y cambiar contraseña
using COT_0.Host.Models;
using COT_0.Host.Services.Perfiles;
using COT_0.Host.Services.Tienda;
using COT_0.Host.Services.Validaciones;

namespace COT_0.Host.Services.Usuarios;

/// <summary>
/// Vuelve a validar todo antes de guardar. Quién puede qué: cada usuario solo su propia cuenta; un admin puede
/// ELIMINAR cualquiera, pero nombre y contraseña solo los cambia su dueño (pide la contraseña actual).
/// </summary>
public class AccountService(PurchaseService purchases)
{
    public static bool CanManage(string userName) => ActiveUser.Is(userName);

    public static bool CanDelete(string userName) => ActiveUser.IsAdmin || ActiveUser.Is(userName);

    public async Task<CredentialError> DeleteAsync(string userName)
    {
        if (!CanDelete(userName)) return CredentialError.NotAllowed;

        var users = await HostAccounts.LoadAsync();
        if (HostAccounts.Find(users, userName) is not { } user) return CredentialError.WrongCredentials;

        users.Remove(user);
        await HostAccounts.SaveAsync(users);
        await purchases.RemoveUserAsync(user.UserName);
        await ProfilePhotos.DeleteAsync(user.UserName);
        if (ActiveUser.Is(user.UserName)) ActiveUser.SignOut();
        return CredentialError.None;
    }

    /// <summary>Mismas reglas que el registro y sin repetir nombre; compras y foto se van con la cuenta.</summary>
    public async Task<CredentialError> RenameAsync(string userName, string newName)
    {
        if (!CanManage(userName)) return CredentialError.NotAllowed;
        var error = CredentialValidator.ValidateUserName(newName);
        if (error != CredentialError.None) return error;

        var users = await HostAccounts.LoadAsync();
        if (HostAccounts.Find(users, userName) is not { } user) return CredentialError.WrongCredentials;
        var other = HostAccounts.Find(users, newName);
        if (other is not null && other != user) return CredentialError.UserTaken;

        var oldName = user.UserName;
        user.UserName = newName;
        await HostAccounts.SaveAsync(users);
        await purchases.RenameUserAsync(oldName, newName);
        await ProfilePhotos.MoveAsync(oldName, newName);
        ActiveUser.Rename(newName);
        return CredentialError.None;
    }

    /// <summary>Pide la contraseña actual; la nueva cumple las reglas vigentes y se escribe igual dos veces.</summary>
    public async Task<CredentialError> ChangePasswordAsync(string userName, string current, string password, string confirmation)
    {
        if (!CanManage(userName)) return CredentialError.NotAllowed;
        if (string.IsNullOrEmpty(current)) return CredentialError.CurrentPasswordEmpty;
        var error = CredentialValidator.ValidateNewPassword(password, confirmation);
        if (error != CredentialError.None) return error;

        var users = await HostAccounts.LoadAsync();
        if (HostAccounts.Find(users, userName) is not { } user || !SecretHasher.Verify(current, user.PasswordHash, user.Salt))
            return CredentialError.WrongCurrentPassword;

        HostAccounts.SetPassword(user, password);
        await HostAccounts.SaveAsync(users);
        return CredentialError.None;
    }
}
