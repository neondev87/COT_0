// Quién hizo ENTRAR, solo en memoria: un usuario (modo HOST) o un administrador (modo ADMIN), nunca los dos
namespace COT_0.Host.Services.Usuarios;

/// <summary>
/// Name decide de quién son el cupo, los tickets, la foto de la firma y los emotes que entran a la sala.
/// AdminName solo abre el punto de venta y la sección ADMINISTRADORES; un admin no compra.
/// </summary>
public static class ActiveUser
{
    /// <summary>Usuario del modo HOST (null en modo ADMIN o sin sesión).</summary>
    public static string? Name { get; private set; }

    /// <summary>Administrador del modo ADMIN (null en modo HOST o sin sesión).</summary>
    public static string? AdminName { get; private set; }

    public static bool IsAdmin => AdminName is not null;

    /// <summary>Cambió quién entró o su nombre (la firma vuelve a pintar su foto).</summary>
    public static event Action? Changed;

    /// <summary>Lo llama el login al terminar ENTRAR en modo HOST (también después de configurar la pregunta).</summary>
    public static void SignIn(string userName) => Set(userName, null);

    /// <summary>Lo llama el login al terminar ENTRAR en modo ADMIN.</summary>
    public static void SignInAdmin(string adminName) => Set(null, adminName);

    /// <summary>Lo llama el login al mostrarse otra vez (SALIR) y USUARIOS al borrar tu propia cuenta.</summary>
    public static void SignOut() => Set(null, null);

    /// <summary>CAMBIAR NOMBRE de tu propia cuenta: la sesión sigue con el nombre nuevo.</summary>
    public static void Rename(string newName)
    {
        if (Name is not null) Set(newName, null);
    }

    /// <summary>true si el nombre es el del usuario que hizo ENTRAR (sin distinguir mayúsculas).</summary>
    public static bool Is(string userName) => string.Equals(Name, userName, StringComparison.OrdinalIgnoreCase);

    private static void Set(string? name, string? adminName)
    {
        Name = name;
        AdminName = adminName;
        Changed?.Invoke();
    }
}
