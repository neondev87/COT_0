using COT_0.Web.Models;
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
            .Order()
            .Select(key => new FlagOption(key, ToLabel(key), GetUrl(key)))
            .ToList();

    public string GetUrl(string key) => $"{_options.FlagsFolder.Trim('/')}/{key}{_options.FlagExtension}";

    // "ciencias-exactas" -> "Ciencias exactas"
    private static string ToLabel(string key)
    {
        var text = key.Replace('-', ' ').Replace('_', ' ');
        return text.Length == 0 ? text : char.ToUpperInvariant(text[0]) + text[1..];
    }
}
