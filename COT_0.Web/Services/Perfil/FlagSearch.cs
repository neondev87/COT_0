using System.Globalization;
using System.Text;
using COT_0.Core.Models;

namespace COT_0.Web.Services.Perfil;

/// <summary>Filtro del buscador de facultades (sin mayúsculas ni acentos)</summary>
public static class FlagSearch
{
    public static IReadOnlyList<FlagOption> Filter(IEnumerable<FlagOption> flags, string? query)
    {
        var q = Normalize(query ?? string.Empty);
        return q.Length == 0
            ? flags.ToList()
            : flags.Where(f => Normalize(f.Label).Contains(q) || f.Key.Contains(q)).ToList();
    }

    private static string Normalize(string text)
    {
        var builder = new StringBuilder();
        foreach (var c in text.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) builder.Append(c);
        }

        return builder.ToString();
    }
}
