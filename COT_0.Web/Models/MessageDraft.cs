using System.Text.RegularExpressions;

namespace COT_0.Web.Models;

// Texto que el invitado está escribiendo, con el límite de caracteres
public sealed partial class MessageDraft(int maxLength)
{
    // Desde este porcentaje del límite el contador se marca en rojo
    private const double WarnRatio = .9;
    private const int MaxRows = 5;

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
    public int Rows => Math.Clamp(_text.Count(c => c == '\n') + 1, 1, MaxRows);

    // Lo que va después de un ":" al final del texto (ej. "hola :ra" → "ra"); null si no se está escribiendo un emote
    public string? EmoteQuery => EmoteQueryPattern().Match(_text) is { Success: true } m ? m.Groups[1].Value : null;

    // Agrega con espacios alrededor, para que el emote quede como palabra suelta
    public void Append(string token) => Text = (_text.Length == 0 || char.IsWhiteSpace(_text[^1]) ? _text : _text + " ") + token + " ";

    // Reemplaza el ":texto" que se estaba escribiendo por el emote elegido
    public void CompleteEmote(string name)
    {
        var match = EmoteQueryPattern().Match(_text);
        if (!match.Success) return;

        var colon = match.Groups[1].Index - 1;
        Text = _text[..colon] + name + " ";
    }

    public void Clear() => _text = string.Empty;

    private string Clamp(string value) => value.Length > MaxLength ? value[..MaxLength] : value;

    [GeneratedRegex(@"(?:^|\s):([^\s:]*)$")]
    private static partial Regex EmoteQueryPattern();
}
