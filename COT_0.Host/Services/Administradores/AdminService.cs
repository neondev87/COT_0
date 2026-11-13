// Administradores en admin.json: entrar (con pregunta secreta la primera vez), crear, regenerar y eliminar con doble confirmación
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Validaciones;

namespace COT_0.Host.Services.Administradores;

/// <summary>
/// La contraseña la genera PasswordGenerator y solo sale en el resultado para mostrarla una vez; aquí solo se guarda su hash.
/// Eliminar pide la respuesta secreta del admin que se va y la contraseña de quien elimina; si se borra al último,
/// admin.json queda vacío y el modo ADMIN vuelve a pedir el primer admin.
/// </summary>
public class AdminService(AdminStore store)
{
    // Admin que ya escribió bien su contraseña pero todavía no configura su pregunta (solo en memoria)
    private string? _pendingQuestion;

    public static int Maximum => AppConfig.Settings.Admin.Maximo;

    public bool TakeRecreatedNotice() => store.TakeRecreatedNotice();

    public async Task<bool> HasAdminsAsync() => (await store.LoadAsync()).Count > 0;

    /// <summary>Solo nombres, en orden de creación; nada más del admin sale de aquí.</summary>
    public async Task<IReadOnlyList<string>> GetNamesAsync() =>
        (await store.LoadAsync()).OrderBy(a => a.CreatedAt).Select(a => a.UserName).ToList();

    /// <summary>
    /// WrongCredentials (genérico) si no existe o la contraseña no coincide; QuestionRequired si entró bien
    /// pero le falta la pregunta secreta (se termina con SetQuestionAsync).
    /// </summary>
    public async Task<AdminError> LoginAsync(string userName, string password)
    {
        _pendingQuestion = null;
        if (ValidateName(userName) != AdminError.None) return AdminError.WrongCredentials;

        var admin = Find(await store.LoadAsync(), userName);
        if (admin is null || !AdminSecrets.VerifyPassword(admin, password)) return AdminError.WrongCredentials;
        if (AdminSecrets.HasQuestion(admin)) return AdminError.None;

        _pendingQuestion = admin.UserName;
        return AdminError.QuestionRequired;
    }

    /// <summary>Solo para el admin que acaba de entrar con LoginAsync y no tenía pregunta.</summary>
    public async Task<CredentialError> SetQuestionAsync(string userName, string questionKey, string answer)
    {
        if (!string.Equals(_pendingQuestion, userName, StringComparison.OrdinalIgnoreCase)) return CredentialError.NotAllowed;
        var error = SecurityAnswerValidator.Validate(questionKey, answer);
        if (error != CredentialError.None) return error;

        var admins = await store.LoadAsync();
        if (Find(admins, userName) is not { } admin) return CredentialError.NotAllowed;
        AdminSecrets.SetQuestion(admin, questionKey, answer);
        await store.SaveAsync(admins);
        _pendingQuestion = null;
        return CredentialError.None;
    }

    /// <summary>Olvida al admin que estaba configurando su pregunta (cambió de modo o salió).</summary>
    public void CancelPendingQuestion() => _pendingQuestion = null;

    /// <summary>Primer admin: solo si admin.json no tiene ninguno (no hace falta sesión).</summary>
    public async Task<AdminPasswordResult> CreateFirstAsync(string userName)
    {
        var admins = await store.LoadAsync();
        return admins.Count > 0 ? AdminPasswordResult.Fail(AdminError.AlreadyCreated) : await AddAsync(admins, userName);
    }

    /// <summary>Otro admin (hasta Admin:Maximo en total): solo con un admin dentro.</summary>
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

        var password = AdminSecrets.SetNewPassword(admin);
        await store.SaveAsync(admins);
        return new AdminPasswordResult(AdminError.None, password);
    }

    /// <summary>Clave de la pregunta del admin (para mostrar su texto al eliminarlo); null si no la tiene o no existe.</summary>
    public async Task<string?> GetQuestionKeyAsync(string userName) =>
        Find(await store.LoadAsync(), userName) is { } admin && AdminSecrets.HasQuestion(admin) ? admin.SecurityQuestion : null;

    /// <summary>
    /// Las dos confirmaciones deben coincidir con sus hashes; si algo falla regresa DeleteRejected (genérico) y no cambia nada.
    /// También sirve para eliminarte a ti mismo (quien llama cierra la sesión).
    /// </summary>
    public async Task<AdminError> DeleteAsync(string userName, AdminDeleteRequest request)
    {
        if (ActiveUser.AdminName is not { } actorName) return AdminError.NotAllowed;
        var admins = await store.LoadAsync();
        if (Find(admins, userName) is not { } target) return AdminError.NotFound;
        if (!AdminSecrets.HasQuestion(target)) return AdminError.QuestionMissing;

        // Se revisan las dos siempre, para no dar pistas de cuál falló
        var answerOk = AdminSecrets.VerifyAnswer(target, request.TargetAnswer);
        var passwordOk = Find(admins, actorName) is { } actor && AdminSecrets.VerifyPassword(actor, request.ActorPassword);
        if (!answerOk || !passwordOk) return AdminError.DeleteRejected;

        admins.Remove(target);
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
        var password = AdminSecrets.SetNewPassword(admin);
        admins.Add(admin);
        await store.SaveAsync(admins);
        return new AdminPasswordResult(AdminError.None, password);
    }

    private static AdminAccount? Find(List<AdminAccount> admins, string userName) =>
        admins.FirstOrDefault(a => string.Equals(a.UserName, userName, StringComparison.OrdinalIgnoreCase));
}
