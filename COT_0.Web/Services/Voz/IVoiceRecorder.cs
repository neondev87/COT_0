using Microsoft.AspNetCore.Components;

namespace COT_0.Web.Services.Voz;

// Micrófono del invitado (Scoped, uno por circuito): voice.js graba (un toque empieza, otro envía) y le avisa a C#
public interface IVoiceRecorder : IAsyncDisposable
{
    VoiceState State { get; }
    TimeSpan Elapsed { get; }
    string? Notice { get; }

    event Action? Changed;

    // Solo después del primer render (usa interop). El composer tiene que tener el botón .voice-mic
    Task AttachAsync(ElementReference composer, string userId);

    void DismissNotice();

    // Suelta el micrófono (si estaba grabando, descarta)
    Task DetachAsync();
}
