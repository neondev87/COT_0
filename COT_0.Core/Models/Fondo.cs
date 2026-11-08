// Un archivo de la carpeta de fondos, tal como se guarda en fondos.json
namespace COT_0.Core.Models;

/// <summary>
/// FileName es solo el nombre (la carpeta sale de Background:Folder). UseStill: el video se muestra
/// como un cuadro fijo en lugar de reproducirse (los pesados empiezan así).
/// </summary>
public record Fondo(string FileName, FondoKind Kind, long SizeBytes, bool UseStill);
