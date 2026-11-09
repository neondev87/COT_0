// Mete al chat los emotes comprados por el usuario que hizo ENTRAR: al abrir la sala y al comprar con la sala abierta
using COT_0.Core.Services;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Sala;
using COT_0.Host.Services.Usuarios;
using COT_0.Web.Services.Emotes;
using Microsoft.Extensions.DependencyInjection;

namespace COT_0.Host.Services.Tienda;

/// <summary>
/// Usa la misma instancia de emotes de la sala (IPurchasedEmotes = IRoomEmotes de la web), así el picker
/// los muestra y los reportes y QUITAR funcionan igual que con los del set. Sin compras, la sala abre normal.
/// </summary>
public class ChatEmoteImport
{
    private readonly WebServerHost _webServer;
    private readonly PurchaseService _purchases;

    public ChatEmoteImport(WebServerHost webServer, PurchaseService purchases)
    {
        _webServer = webServer;
        _purchases = purchases;
        purchases.Purchased += OnPurchased;
    }

    private IPurchasedEmotes Target => _webServer.Services.GetRequiredService<IPurchasedEmotes>();

    /// <summary>Al abrir la sala: reemplaza los comprados por los de quien entró (sin preguntar nada).</summary>
    public async Task ImportAsync()
    {
        try
        {
            var emotes = ActiveUser.Name is { } name ? (await _purchases.GetAsync(name)).Emotes : [];
            Target.SetPurchased(emotes.Select(EmoteMapping.ToRoom).ToList());
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            // Si compras.json no se puede leer, la sala abre igual sin comprados
        }
    }

    /// <summary>Al cerrar la sala: la siguiente sala vuelve a importar los de quien la abra.</summary>
    public void Clear() => Target.SetPurchased([]);

    private void OnPurchased(string userName)
    {
        var room = _webServer.Services.GetRequiredService<IRoomService>();
        if (room.IsOpen && string.Equals(userName, ActiveUser.Name, StringComparison.OrdinalIgnoreCase)) _ = ImportAsync();
    }
}
