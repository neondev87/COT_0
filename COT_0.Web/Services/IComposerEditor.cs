using COT_0.Core.Models;
using COT_0.Web.Models;
using Microsoft.AspNetCore.Components;

namespace COT_0.Web.Services;

// Cuadro de escritura (Scoped, uno por circuito): composer.js muestra los emotes como imagen y le pasa a C# el texto con las claves
public interface IComposerEditor : IAsyncDisposable
{
    // Cambió el texto del borrador (para avisar "escribiendo" y volver a pintar)
    event Action? Changed;

    // Solo después del primer render (usa interop)
    Task AttachAsync(ElementReference editor, MessageDraft draft);

    // Desde la carita va en el cursor; desde ":" reemplaza lo que se estaba escribiendo
    Task InsertEmoteAsync(RoomEmote emote, bool replaceQuery);

    // Vacía el cuadro (después de enviar)
    Task ClearAsync();

    // Suelta el cuadro (al salir de la página)
    Task DetachAsync();
}
