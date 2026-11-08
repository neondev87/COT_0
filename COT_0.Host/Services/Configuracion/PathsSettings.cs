// Sección "Paths" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>Única ruta absoluta de la PC: carpeta con musica, portadas, fondos, gifs y manual (fuera del repo).</summary>
public record PathsSettings(string MediaRoot);
