using COT_0.Core.Models;

namespace COT_0.Web.Services.Emotes;

// Menú de un emote del chat (Scoped): se abre con clic derecho o manteniendo presionado
public interface IEmoteActions
{
    RoomEmote? Target { get; }

    // Resultado del último reporte (para el aviso); NoticeKey cambia con cada reporte
    bool? LastReportSent { get; }
    int NoticeKey { get; }

    event Action? Changed;

    void Open(RoomEmote emote);
    void Close();
    void Report(User reporter);
}
