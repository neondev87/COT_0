// Contenido de fondos.json: catálogo de la última lectura y el fondo elegido
namespace COT_0.HostWinUI.Models;

/// <summary>
/// SelectedFile: null = nunca elegiste (se usa Background:DefaultFile); "" = sin fondo; si no, el nombre del archivo.
/// </summary>
public class FondosGuardados
{
    public string? SelectedFile { get; set; }

    public List<Fondo> Items { get; set; } = [];
}
