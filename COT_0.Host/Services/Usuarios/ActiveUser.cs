// Usuario que hizo ENTRAR, solo en memoria: lo usa la tienda para comprar e importar sus emotes al chat
namespace COT_0.Host.Services.Usuarios;

/// <summary>
/// La interfaz nunca lo muestra (regla: dentro de la app nadie sabe qué usuario entró);
/// solo decide de quién son el cupo, los tickets y los emotes que entran a la sala.
/// </summary>
public static class ActiveUser
{
    public static string? Name { get; private set; }

    /// <summary>Lo llama el login al terminar ENTRAR (también después de configurar la pregunta).</summary>
    public static void SignIn(string userName) => Name = userName;

    /// <summary>Lo llama el login al mostrarse otra vez (SALIR).</summary>
    public static void SignOut() => Name = null;
}
