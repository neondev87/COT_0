// Sección "Store" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>
/// Tienda de emotes: cupo por usuario (DailyLimit cada CooldownHours), emotes por página de 7TV,
/// tamaño de las miniaturas, dónde y cómo se guardan los tickets en PDF (copia local en GeneratedTicketsFolder
/// y descarga en una carpeta del Escritorio), cuánto dura el sello de compra antes del panel y los precios de muestra
/// en centavos de dólar (la mayoría entre PriceMinCents y PriceMaxCents; PremiumPercent de cada 100 entre PremiumMin/MaxCents).
/// </summary>
public record StoreSettings(
    int DailyLimit,
    int CooldownHours,
    int PageSize,
    int ThumbnailSize,
    string TicketsFolder,
    string GeneratedTicketsFolder,
    string TicketFileFormat,
    string TicketNumberFormat,
    string TicketDateFormat,
    string WaitFormat,
    string TicketFont,
    int PurchaseEffectMs,
    int PriceMinCents,
    int PriceMaxCents,
    int PremiumMinCents,
    int PremiumMaxCents,
    int PremiumPercent,
    string PriceFormat);
