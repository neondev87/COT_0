// Contenido de fondos.json: catálogo de la última lectura y el fondo elegido
using COT_0.Core.Models;
namespace COT_0.Host.Models;

/// <summary>
/// SelectedFile: null = nunca elegiste (se usa Background:DefaultFile); "" = sin fondo; si no, el nombre del archivo.
/// </summary>
public class FondosGuardados
{
    public string? SelectedFile { get; set; }

    public List<Fondo> Items { get; set; } = [];
}
