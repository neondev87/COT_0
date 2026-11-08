namespace COT_0.Web.Services.Imagenes;

/// <summary>Una imagen viaja como ChatMessage con Kind = Image e ImageUrl; el render todavía la reconoce por este texto reservado, igual que VoiceNote</summary>
public static class ImageNote
{
    private const string Prefix = "\u0001image:";

    public static string ToText(string imageId) => Prefix + imageId;

    public static bool IsImage(string text) => text.StartsWith(Prefix, StringComparison.Ordinal);

    public static string? GetImageId(string text) => IsImage(text) ? text[Prefix.Length..] : null;
}
