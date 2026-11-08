namespace COT_0.Web.Models;

/// <summary>Paso del manual del invitado; Focus de cada aviso = parte de la vista que se resalta</summary>
public class ManualStep
{
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public ManualView View { get; set; }
    public List<ManualNotice> Notices { get; set; } = [];
}
