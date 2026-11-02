namespace COT_0.Web.Models;

// Fragmento de un mensaje: texto plano o un emote
public record MessagePart(string Text, Emote? Emote);
