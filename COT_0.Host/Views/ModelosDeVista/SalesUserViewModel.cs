// Un renglón de la lista de usuarios del PUNTO DE VENTA: nombre y cuántos tickets y emotes lleva (o TODOS)
namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Name null es la opción TODOS. Nunca trae contraseña, hash ni pregunta secreta.</summary>
public sealed class SalesUserViewModel(string? name, string title, string summary)
{
    public string? Name { get; } = name;

    public string Title { get; } = title;

    public string Summary { get; } = summary;

    public bool IsAll => Name is null;
}
