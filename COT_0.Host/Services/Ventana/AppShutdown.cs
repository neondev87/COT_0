// Apagado ordenado de la app: lo llama CloseGuard después de que confirmaste salir
using COT_0.Core.Services;
using COT_0.Host.Services.Fondos;
using COT_0.Host.Services.Manual;
using COT_0.Host.Services.Musica;
using COT_0.Host.Services.Sala;
using COT_0.Host.Services.Tiempo;

namespace COT_0.Host.Services.Ventana;

/// <summary>
/// StopAsync (con la ventana abierta): deja de recibir eventos, detiene los reproductores (sueltan su archivo) y apaga
/// sala, túnel y Kestrel. Release (ya cerrada la ventana): suelta el job de cloudflared.
/// Los MediaPlayer no se liberan con Dispose: sus MediaPlayerElement y el Unloaded de ManualPage los siguen tocando
/// después de cerrar la ventana; liberarlos ahí colgaba el cierre o tronaba (0xC000027B). Windows los suelta al salir.
/// </summary>
public class AppShutdown(
    RoomSession session,
    ITunnelService tunnel,
    ClockService clock,
    MusicService music,
    FondoPlayer background,
    ManualVideo manualVideo,
    ManualSound manualSound)
{
    public async Task StopAsync()
    {
        clock.Stop();
        music.Stop();
        background.Stop();
        manualVideo.Stop();
        manualSound.Stop();
        await session.ShutdownAsync();
        // Lo que todavía llegue de la web o de los reproductores ya no toca la interfaz
        UiThread.Detach();
    }

    public void Release() => (tunnel as IDisposable)?.Dispose();
}
