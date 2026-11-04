namespace COT_0.Web.Models;

// Texto que el invitado está escribiendo, con el límite de caracteres
public sealed class MessageDraft(int maxLength)
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

    public void Append(string token) => Text = _text.Length == 0 || char.IsWhiteSpace(_text[^1]) ? _text + token : $"{_text} {token}";

    public void Clear() => _text = string.Empty;

    private string Clamp(string value) => value.Length > MaxLength ? value[..MaxLength] : value;
}
