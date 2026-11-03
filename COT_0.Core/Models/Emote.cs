namespace COT_0.Core.Models;

// Emote animado de wwwroot/emotes; se escribe como :codigo:
public record Emote(string Code, string Url)
{
    public string Token => $":{Code}:";
}
