using COT_0.Web.Models;

namespace COT_0.Web.Services;

// Perfil del invitado en localStorage (solo después del primer render: usa interop)
public interface IGuestStore
{
    Task<StoredGuest?> LoadAsync();
    Task SaveAsync(StoredGuest guest);
    Task ClearAsync();
}
