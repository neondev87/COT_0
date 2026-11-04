using Microsoft.AspNetCore.Components;

namespace COT_0.Web.Services;

// Lado navegador del chat (Scoped): Enter para enviar, scroll automático y hora local del invitado
public interface IChatBrowser
{
    // Solo después del primer render (usa interop)
    Task AttachAsync(ElementReference chatPage);

    string FormatTime(DateTime utc);
}
