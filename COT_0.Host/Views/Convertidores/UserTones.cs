// Elige el tono de la inicial de un usuario a partir de su nombre (el mismo en su tarjeta y en la firma)
namespace COT_0.Host.Views.Convertidores;

/// <summary>Número de 0 a 4 para AvatarTone{n}Brush (AvatarTones); no cambia mientras el nombre sea el mismo.</summary>
public static class UserTones
{
    private const int ToneCount = 5;

    public static int For(string name) => name.ToLowerInvariant().Sum(c => c) % ToneCount;
}
