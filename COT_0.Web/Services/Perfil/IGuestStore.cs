namespace COT_0.Web.Services.Perfil;

/// <summary>Perfil del invitado en localStorage (solo después del primer render: usa interop)</summary>
public interface IGuestStore
{
    Task<StoredGuest?> LoadAsync();
    Task SaveAsync(StoredGuest guest);
    Task ClearAsync();
}
