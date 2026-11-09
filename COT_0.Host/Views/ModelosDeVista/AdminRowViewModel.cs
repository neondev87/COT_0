// Un admin en la sección ADMINISTRADORES: solo su nombre y si se puede borrar
namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>No se puede borrar a ti mismo ni al último admin (el servicio también lo revisa).</summary>
public sealed class AdminRowViewModel(string name, bool isYou, bool canDelete)
{
    public string Name { get; } = name;

    public bool IsYou { get; } = isYou;

    public bool CanDelete { get; } = canDelete;
}
