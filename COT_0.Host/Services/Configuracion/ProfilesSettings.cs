// Sección "Profiles" de appsettings.json
namespace COT_0.Host.Services.Configuracion;

/// <summary>Fotos de perfil: lado máximo del cuadrado guardado, extensiones que se pueden elegir y nombre del archivo ({0} = usuario).</summary>
public record ProfilesSettings(int PhotoMaxPixels, string[] PhotoExtensions, string PhotoFileFormat);
