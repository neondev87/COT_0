// Un admin en la sección ADMINISTRADORES: solo su nombre y si eres tú
namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Cualquier admin se puede eliminar (también tú y el último) con las dos confirmaciones.</summary>
public sealed class AdminRowViewModel(string name, bool isYou)
{
    public string Name { get; } = name;

    public bool IsYou { get; } = isYou;
}
