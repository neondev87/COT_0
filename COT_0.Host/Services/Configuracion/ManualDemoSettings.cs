// Sección "Manual:Demo" de appsettings.json: datos ficticios de las vistas de ejemplo del manual
namespace COT_0.Host.Services.Configuracion;

/// <summary>
/// Nada sale de la red ni de la sala real: usuario del login, código, tiempo y enlace de la sala,
/// invitados, emotes comprados con reportes y quitados, canciones (PlayingSong = la que suena) y volumen de muestra (0 a 100).
/// </summary>
public record ManualDemoSettings(
    string User,
    string Code,
    string Elapsed,
    string JoinUrl,
    IReadOnlyList<ManualDemoGuest> Guests,
    int EmoteCount,
    IReadOnlyList<ManualDemoEmote> Reports,
    IReadOnlyList<string> Removed,
    IReadOnlyList<ManualDemoSong> Songs,
    int PlayingSong,
    double Volume);
