using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

// Facultades de Chat:FlagNames; el escudo sale de wwwroot/img/flags/{clave}.png si existe
public class FlagService(IWebHostEnvironment env, IOptions<ChatOptions> options) : IFlagService
{
    private readonly ChatOptions _options = options.Value;

    public IReadOnlyList<FlagOption> GetFlags() =>
        _options.FlagNames
            .Select(f => new FlagOption(f.Key, f.Value, GetUrl(f.Key)))
            .OrderBy(f => f.Label, StringComparer.CurrentCulture)
            .ToList();

    // Vacía si todavía no hay imagen (la vista muestra un marcador)
    public string GetUrl(string key)
    {
        var path = $"{_options.FlagsFolder.Trim('/')}/{key}{_options.FlagExtension}";
        return env.WebRootFileProvider.GetFileInfo(path).Exists ? path : string.Empty;
    }
}
