using COT_0.Core.Models;

namespace COT_0.Core.Services;

// Banderas de facultad de wwwroot/img/flags
public interface IFlagService
{
    IReadOnlyList<FlagOption> GetFlags();
    string GetUrl(string key);
}
