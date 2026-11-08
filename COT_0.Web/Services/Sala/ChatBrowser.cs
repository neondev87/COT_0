using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace COT_0.Web.Services.Sala;

public sealed class ChatBrowser(IJSRuntime js, IOptions<ChatOptions> options, ILogger<ChatBrowser> logger) : IChatBrowser
{
    // Hasta saber la zona del navegador se usa la del servidor
    private TimeZoneInfo _zone = TimeZoneInfo.Local;

    public async Task AttachAsync(ElementReference chatPage)
    {
        try
        {
            var zone = await js.InvokeAsync<BrowserZone>("cotChat.attach", chatPage, options.Value.EmoteHoldMs);
            _zone = FindZone(zone);
        }
        catch (JSException ex)
        {
            logger.LogDebug(ex, "No se pudo preparar el chat en el navegador.");
        }
    }

    public string FormatTime(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), _zone).ToString(options.Value.TimeFormat);

    // Zona IANA (ej. America/Monterrey); si el servidor no la conoce, el desfase en minutos
    private static TimeZoneInfo FindZone(BrowserZone zone)
    {
        if (!string.IsNullOrEmpty(zone.Id) && TimeZoneInfo.TryFindSystemTimeZoneById(zone.Id, out var found)) return found;

        var offset = TimeSpan.FromMinutes(zone.OffsetMinutes);
        return TimeZoneInfo.CreateCustomTimeZone("browser", offset, "browser", "browser");
    }

    private sealed record BrowserZone(string? Id, int OffsetMinutes);
}
