// Glifos de Segoe Fluent Icons para los controles de música (reemplazan los DataTrigger que cambiaban el Path)
namespace COT_0.Host.Views.Convertidores;

/// <summary>Se usan con x:Bind: el icono cambia según el estado del reproductor.</summary>
public static class MusicGlyphs
{
    private const string Play = "\uE768";
    private const string Pause = "\uE769";
    private const string NoteOn = "\uE8D6";
    private const string NoteMuted = "\uE74F";
    private const string SpeakerOn = "\uE767";
    private const string SpeakerZero = "\uE992";
    private const string SpeakerLow = "\uE993";
    private const string SpeakerHigh = "\uE995";
    private const double HighFrom = 50;

    public static string PlayPause(bool isPlaying) => isPlaying ? Pause : Play;

    public static string Note(bool isMuted) => isMuted ? NoteMuted : NoteOn;

    /// <summary>Bocina de la firma por rangos: mudo (tachada), en 0, bajo (menos de 50) o alto.</summary>
    public static string Volume(bool isMuted, double volume) => isMuted
        ? NoteMuted
        : volume < 0.5 ? SpeakerZero : volume < HighFrom ? SpeakerLow : SpeakerHigh;

    /// <summary>Bocina de SILENCIAR en el manual (tachada con el manual en silencio).</summary>
    public static string Speaker(bool isMuted) => isMuted ? NoteMuted : SpeakerOn;
}
