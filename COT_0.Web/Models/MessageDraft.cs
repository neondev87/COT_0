using System.Text.RegularExpressions;

namespace COT_0.Web.Models;

// Texto que el invitado está escribiendo, con el límite de caracteres
public sealed partial class MessageDraft(int maxLength)
{
    // Desde este porcentaje del límite el contador se marca en rojo
    private const double WarnRatio = .9;

    private string _text = string.Empty;

    public int MaxLength { get; } = maxLength;

    public string Text
    {
        get => _text;
        set => _text = Clamp(value ?? string.Empty);
    }

    public int Length => _text.Length;
    public bool CanSend => !string.IsNullOrWhiteSpace(_text);
    public bool IsNearLimit => Length >= MaxLength * WarnRatio;

    // Lo que va después de un ":" al final del texto (ej. "hola :ra" → "ra"); null si no se está escribiendo un emote
    public string? EmoteQuery => EmoteQueryPattern().Match(_text) is { Success: true } m ? m.Groups[1].Value : null;

    public void Clear() => _text = string.Empty;

    private string Clamp(string value) => value.Length > MaxLength ? value[..MaxLength] : value;

    [GeneratedRegex(@"(?:^|\s):([^\s:]*)$")]
    private static partial Regex EmoteQueryPattern();
}
