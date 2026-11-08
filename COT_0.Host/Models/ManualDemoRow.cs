// Fila ficticia de las vistas de ejemplo del manual (emote reportado o quitado, canción de la biblioteca)
namespace COT_0.Host.Models;

/// <summary>Título, detalle ya formateado (reportes o duración) y si va marcada (la canción que suena).</summary>
public sealed record ManualDemoRow(string Title, string Detail, bool IsMarked)
{
    /// <summary>Primera letra para el recuadro que reemplaza la miniatura (sin descargar nada de 7TV).</summary>
    public string Initial => Title.Length > 0 ? Title[..1].ToUpperInvariant() : string.Empty;
}
