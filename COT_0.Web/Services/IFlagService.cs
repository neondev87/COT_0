using COT_0.Web.Models;

namespace COT_0.Web.Services;

// Banderas de facultad de wwwroot/img/flags
public interface IFlagService
{
    IReadOnlyList<FlagOption> GetFlags();
    string GetUrl(string key);
}
