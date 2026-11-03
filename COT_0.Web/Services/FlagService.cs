using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

public class FlagService(IWebHostEnvironment env, IOptions<ChatOptions> options) : IFlagService
{
    private readonly ChatOptions _options = options.Value;

    public IReadOnlyList<FlagOption> GetFlags() =>
        env.WebRootFileProvider.GetDirectoryContents(_options.FlagsFolder.Trim('/'))
            .Where(f => !f.IsDirectory && f.Name.EndsWith(_options.FlagExtension, StringComparison.OrdinalIgnoreCase))
            .Select(f => Path.GetFileNameWithoutExtension(f.Name))
            .Select(key => new FlagOption(key, ToLabel(key), GetUrl(key)))
            .OrderBy(f => f.Label, StringComparer.CurrentCulture)
            .ToList();

    public string GetUrl(string key) => $"{_options.FlagsFolder.Trim('/')}/{key}{_options.FlagExtension}";

    // Nombre de Chat:FlagNames; si no está, "ciencias-exactas" -> "Ciencias exactas"
    private string ToLabel(string key)
    {
        if (_options.FlagNames.TryGetValue(key, out var name)) return name;

        var text = key.Replace('-', ' ').Replace('_', ' ');
        return text.Length == 0 ? text : char.ToUpperInvariant(text[0]) + text[1..];
    }
}
