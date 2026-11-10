// Sección "Store" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>
/// Tienda de emotes: cupo por usuario (DailyLimit cada CooldownHours), emotes por página de 7TV,
/// tamaño de las miniaturas, dónde y cómo se guardan los tickets en PDF (carpeta en el Escritorio)
/// y cuánto dura el efecto de compra antes de preguntar por el PDF.
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
    string TicketFont,
    int PurchaseEffectMs);
