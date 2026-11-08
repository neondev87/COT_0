using COT_0.Core.Models;

namespace COT_0.Core.Services;

/// <summary>Banderas de facultad de wwwroot/img/flags</summary>
public interface IFlagService
{
    IReadOnlyList<FlagOption> GetFlags();
    string GetUrl(string key);
}
