namespace COT_0.Web.Services.Voz;

// Una nota de voz viaja como ChatMessage con Kind = Voice y AudioUrl; el render todavía la reconoce por este texto reservado
public static class VoiceNote
{
    private const string Prefix = "\u0001voice:";

    public static string ToText(string clipId) => Prefix + clipId;

    public static bool IsVoice(string text) => text.StartsWith(Prefix, StringComparison.Ordinal);

    public static string? GetClipId(string text) => IsVoice(text) ? text[Prefix.Length..] : null;
}
