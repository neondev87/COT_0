using COT_0.Core.Models;
using COT_0.Core.Services;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Perfil;

/// <summary>Facultades de Chat:FlagNames; el escudo sale de wwwroot/img/flags/{clave}.png si existe</summary>
public class FlagService(IWebHostEnvironment env, IOptions<ChatOptions> options) : IFlagService
{
    private readonly ChatOptions _options = options.Value;

    public IReadOnlyList<FlagOption> GetFlags() =>
        _options.FlagNames
            .Select(f => new FlagOption(f.Key, f.Value, GetUrl(f.Key)))
            .OrderBy(f => PriorityOf(f.Key))
            .ThenBy(f => f.Label, StringComparer.CurrentCulture)
            .ToList();

    // Posición en Chat:FlagPriority; las que no están van al final
    private int PriorityOf(string key)
    {
        var index = Array.IndexOf(_options.FlagPriority, key);
        return index < 0 ? int.MaxValue : index;
    }

    // Vacía si todavía no hay imagen (la vista muestra un marcador)
    public string GetUrl(string key)
    {
        var path = $"{_options.FlagsFolder.Trim('/')}/{key}{_options.FlagExtension}";
        return env.WebRootFileProvider.GetFileInfo(path).Exists ? path : string.Empty;
    }
}
