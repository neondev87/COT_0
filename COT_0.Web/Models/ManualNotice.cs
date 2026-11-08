namespace COT_0.Web.Models;

/// <summary>Aviso de Lain dentro de un paso del manual; Focus es la parte de la vista que se resalta.</summary>
public class ManualNotice
{
    public string Text { get; set; } = string.Empty;
    public string? Focus { get; set; }
}
