using COT_0.Core.Models;

namespace COT_0.Core.Services;

/// <summary>
/// Catálogo de la carpeta de fondos (imágenes y videos) sin tipos de vista. Lo implementa el Host
/// (FondoCatalog) porque la carpeta y las extensiones salen de su appsettings.json.
/// </summary>
public interface IBackgroundCatalog
{
    // Carpeta resuelta (Paths:MediaRoot + Background:Folder)
    string Folder { get; }
    bool FolderExists { get; }

    // Ruta completa del archivo de un fondo
    string PathOf(Fondo fondo);

    // Fondos ordenados por nombre; vacío si la carpeta no existe
    Task<IReadOnlyList<Fondo>> ScanAsync();
}
