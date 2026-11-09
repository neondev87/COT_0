// Sección "Store" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>
/// Tienda de emotes: cupo por usuario (DailyLimit cada CooldownHours), emotes por página de 7TV,
/// tamaño de las miniaturas y dónde y cómo se guardan los tickets en PDF (carpeta en el Escritorio).
/// </summary>
public record StoreSettings(
    int DailyLimit,
    int CooldownHours,
    int PageSize,
    int ThumbnailSize,
    string TicketsFolder,
    string TicketFileFormat,
    string TicketNumberFormat,
    string TicketDateFormat,
    string WaitFormat,
    string TicketFont);
