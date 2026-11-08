// Sección "Inputs" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>Largo máximo de todo lo que escribe el host; el set de 7TV tiene el suyo porque su ID mide 24 o 26.</summary>
public record InputsSettings(int MaxCaracteres, int MaxEmoteSetCaracteres);
