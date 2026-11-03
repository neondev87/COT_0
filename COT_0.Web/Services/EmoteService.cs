using System.Text.RegularExpressions;
using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

public partial class EmoteService : IEmoteService
{
    private readonly Lazy<Dictionary<string, Emote>> _emotes;

    public EmoteService(IWebHostEnvironment env, IOptions<ChatOptions> options)
    {
        _emotes = new(() => Load(env, options.Value));
    }

    public IReadOnlyList<Emote> GetEmotes() => _emotes.Value.Values.OrderBy(e => e.Code).ToList();

    public IReadOnlyList<MessagePart> Parse(string text)
    {
        var parts = new List<MessagePart>();
        var last = 0;

        foreach (Match match in CodePattern().Matches(text))
        {
            if (!_emotes.Value.TryGetValue(match.Groups[1].Value, out var emote)) continue;

            if (match.Index > last) parts.Add(new MessagePart(text[last..match.Index], null));
            parts.Add(new MessagePart(match.Value, emote));
            last = match.Index + match.Length;
        }

        if (last < text.Length) parts.Add(new MessagePart(text[last..], null));
        return parts;
    }

    // Lee la carpeta de emotes una sola vez (agregar emotes requiere reiniciar)
    private static Dictionary<string, Emote> Load(IWebHostEnvironment env, ChatOptions options)
    {
        var folder = options.EmotesFolder.Trim('/');

        return env.WebRootFileProvider.GetDirectoryContents(folder)
            .Where(f => !f.IsDirectory && options.EmoteExtensions.Contains(Path.GetExtension(f.Name), StringComparer.OrdinalIgnoreCase))
            .Select(f => new Emote(Path.GetFileNameWithoutExtension(f.Name), $"{folder}/{f.Name}"))
            .DistinctBy(e => e.Code, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(e => e.Code, StringComparer.OrdinalIgnoreCase);
    }

    [GeneratedRegex(@":([\w\-]+):")]
    private static partial Regex CodePattern();
}
