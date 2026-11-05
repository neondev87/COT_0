namespace COT_0.Web.Models;

// Vista real de COT_0.Web que acompaña un paso del manual (en modo demostración)
public enum ManualView
{
    Join,
    Chat,
    Emotes,
    Music,
    Exit,
}

// Paso del manual del invitado; Focus de cada aviso = parte de la vista que se resalta
public class ManualStep
{
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public ManualView View { get; set; }
    public List<ManualNotice> Notices { get; set; } = [];
}

public class ManualNotice
{
    public string Text { get; set; } = string.Empty;
    public string? Focus { get; set; }
}

// Aviso de Lain listo para pintar: Style trae --in y --out (cuándo entra y sale)
public sealed record ManualCue(string Text, string Style, bool Leaves);
