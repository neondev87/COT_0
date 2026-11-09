// Administradores en admin.json: entrar, crear (el primero sin sesión; los demás solo un admin), regenerar y borrar
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Validaciones;

namespace COT_0.Host.Services.Administradores;

/// <summary>
/// La contraseña la genera PasswordGenerator y solo sale en el resultado para mostrarla una vez; aquí solo se guarda su hash.
/// No hay forma de leerla ni recuperarla: si se pierde, otro admin la regenera (o se borra admin.json si era el único).
/// </summary>
public class AdminService(AdminStore store)
{
    public static int Maximum => AppConfig.Settings.Admin.Maximo;

    public bool TakeRecreatedNotice() => store.TakeRecreatedNotice();

    public async Task<bool> HasAdminsAsync() => (await store.LoadAsync()).Count > 0;

    /// <summary>Solo nombres, en orden de creación; nada más del admin sale de aquí.</summary>
    public async Task<IReadOnlyList<string>> GetNamesAsync() =>
        (await store.LoadAsync()).OrderBy(a => a.CreatedAt).Select(a => a.UserName).ToList();

    /// <summary>Mensaje genérico (WrongCredentials) si no existe o la contraseña no coincide.</summary>
    public async Task<AdminError> LoginAsync(string userName, string password)
    {
        var error = ValidateName(userName);
        if (error != AdminError.None || string.IsNullOrEmpty(password) || InputLimits.IsTooLong(password)) return AdminError.WrongCredentials;

        var admin = Find(await store.LoadAsync(), userName);
        return admin is not null && SecretHasher.Verify(password, admin.PasswordHash, admin.Salt) ? AdminError.None : AdminError.WrongCredentials;
    }

    /// <summary>Primer admin: solo si admin.json no tiene ninguno (no hace falta sesión).</summary>
    public async Task<AdminPasswordResult> CreateFirstAsync(string userName)
    {
        var admins = await store.LoadAsync();
        return admins.Count > 0 ? AdminPasswordResult.Fail(AdminError.AlreadyCreated) : await AddAsync(admins, userName);
    }

    /// <summary>Otro admin (hasta Admin:Maximo): solo con un admin dentro.</summary>
    public async Task<AdminPasswordResult> CreateAsync(string userName)
    {
        if (!ActiveUser.IsAdmin) return AdminPasswordResult.Fail(AdminError.NotAllowed);
        var admins = await store.LoadAsync();
        return admins.Count >= Maximum ? AdminPasswordResult.Fail(AdminError.Full) : await AddAsync(admins, userName);
    }

    /// <summary>Contraseña nueva para ese admin; la anterior deja de servir al guardar.</summary>
    public async Task<AdminPasswordResult> RegenerateAsync(string userName)
    {
        if (!ActiveUser.IsAdmin) return AdminPasswordResult.Fail(AdminError.NotAllowed);
        var admins = await store.LoadAsync();
        if (Find(admins, userName) is not { } admin) return AdminPasswordResult.Fail(AdminError.NotFound);

        var password = SetPassword(admin);
        await store.SaveAsync(admins);
        return new AdminPasswordResult(AdminError.None, password);
    }

    /// <summary>Nunca deja admin.json sin admins.</summary>
    public async Task<AdminError> DeleteAsync(string userName)
    {
        if (!ActiveUser.IsAdmin) return AdminError.NotAllowed;
        var admins = await store.LoadAsync();
        if (Find(admins, userName) is not { } admin) return AdminError.NotFound;
        if (admins.Count <= 1) return AdminError.LastAdmin;

        admins.Remove(admin);
        await store.SaveAsync(admins);
        return AdminError.None;
    }

    /// <summary>Mismas reglas que el usuario del modo HOST (solo letras, tipo "Erick", hasta Inputs:MaxCaracteres).</summary>
    public static AdminError ValidateName(string userName) => CredentialValidator.ValidateUserName(userName) switch
    {
        CredentialError.None => AdminError.None,
        CredentialError.UserEmpty => AdminError.UserEmpty,
        CredentialError.UserTooLong => AdminError.UserTooLong,
        _ => AdminError.UserInvalid,
    };

    private async Task<AdminPasswordResult> AddAsync(List<AdminAccount> admins, string userName)
    {
        var error = ValidateName(userName);
        if (error != AdminError.None) return AdminPasswordResult.Fail(error);
        if (Find(admins, userName) is not null) return AdminPasswordResult.Fail(AdminError.Taken);

        var admin = new AdminAccount { UserName = userName, CreatedAt = DateTime.UtcNow };
        var password = SetPassword(admin);
        admins.Add(admin);
        await store.SaveAsync(admins);
        return new AdminPasswordResult(AdminError.None, password);
    }

    private static string SetPassword(AdminAccount admin)
    {
        var password = PasswordGenerator.Create();
        var hashed = SecretHasher.Create(password);
        admin.PasswordHash = hashed.Hash;
        admin.Salt = hashed.Salt;
        return password;
    }

    private static AdminAccount? Find(List<AdminAccount> admins, string userName) =>
        admins.FirstOrDefault(a => string.Equals(a.UserName, userName, StringComparison.OrdinalIgnoreCase));
}
