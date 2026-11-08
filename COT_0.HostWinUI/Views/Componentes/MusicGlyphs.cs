// Glifos de Segoe Fluent Icons para los controles de música (reemplazan los DataTrigger que cambiaban el Path)
namespace COT_0.HostWinUI.Views.Componentes;

/// <summary>Se usan con x:Bind: el icono cambia según el estado del reproductor.</summary>
public static class MusicGlyphs
{
    private const string Play = "\uE768";
    private const string Pause = "\uE769";
    private const string NoteOn = "\uE8D6";
    private const string NoteMuted = "\uE74F";
    private const string SpeakerOn = "\uE767";

    public static string PlayPause(bool isPlaying) => isPlaying ? Pause : Play;

    public static string Note(bool isMuted) => isMuted ? NoteMuted : NoteOn;

    /// <summary>Bocina de SILENCIAR en el manual (tachada con el manual en silencio).</summary>
    public static string Speaker(bool isMuted) => isMuted ? NoteMuted : SpeakerOn;
}
