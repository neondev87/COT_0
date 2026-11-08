using Microsoft.AspNetCore.Components;

namespace COT_0.Web.Services.Sala;

/// <summary>Lado navegador del chat (Scoped): scroll automático y hora local del invitado</summary>
public interface IChatBrowser
{
    // Solo después del primer render (usa interop)
    Task AttachAsync(ElementReference chatPage);

    string FormatTime(DateTime utc);
}
