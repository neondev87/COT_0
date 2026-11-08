// Una canción de la carpeta de música, lista para mostrar en la biblioteca
namespace COT_0.Host.Models;

/// <summary>Título = nombre del archivo; CoverPath es null si no hay portada.</summary>
public record Song(string Path, string Title, string Duration, string? CoverPath);
