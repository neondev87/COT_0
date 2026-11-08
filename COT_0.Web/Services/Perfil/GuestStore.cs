using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Perfil;

// localStorage cifrado (ProtectedLocalStorage). Si el navegador no deja guardar
// (Safari privado, almacenamiento lleno, circuito caído) se sigue sin recordar nada.
public class GuestStore(ProtectedLocalStorage storage, IOptions<ChatOptions> options, ILogger<GuestStore> logger)
    : IGuestStore
{
    private readonly string _key = options.Value.GuestStorageKey;

    public async Task<StoredGuest?> LoadAsync()
    {
        try
        {
            var result = await storage.GetAsync<StoredGuest>(_key);
            return result.Success ? result.Value : null;
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "No se pudo leer el perfil guardado.");
            return null;
        }
    }

    public async Task SaveAsync(StoredGuest guest)
    {
        try
        {
            await storage.SetAsync(_key, guest);
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "No se pudo guardar el perfil.");
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            await storage.DeleteAsync(_key);
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "No se pudo borrar el perfil guardado.");
        }
    }
}
