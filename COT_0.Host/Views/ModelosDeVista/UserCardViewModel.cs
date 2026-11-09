// Tarjeta de un usuario registrado en USUARIOS: inicial con color y nombre (nunca contraseña, pregunta ni respuesta)
namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Tone elige el pincel AvatarTone{n}Brush (Convertidores/AvatarTones), igual que la lista de conectados.</summary>
public sealed class UserCardViewModel(string name, int position)
{
    private const int ToneCount = 5;
    private const string UnknownInitial = "?";

    public string Name { get; } = name;

    public string Initial { get; } = name.Length > 0 ? char.ToUpperInvariant(name[0]).ToString() : UnknownInitial;

    public int Tone { get; } = position % ToneCount;
}
