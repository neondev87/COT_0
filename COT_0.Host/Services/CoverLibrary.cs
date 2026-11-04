using System.IO;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Portadas de Audio:CoversFolder: una por nombre (la primera extensión de Audio:CoverExtensions gana)
public class CoverLibrary
{
    private readonly string _folder;
    private readonly string[] _extensions;

    public CoverLibrary(IConfiguration configuration)
    {
        _folder = configuration["Audio:CoversFolder"] ?? string.Empty;
        _extensions = configuration.GetSection("Audio:CoverExtensions").Get<string[]>() ?? [];
    }

    // Archivo de portada por nombre sin extensión, ordenado por nombre
    public IReadOnlyDictionary<string, string> GetCovers()
    {
        var covers = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(_folder) || !Directory.Exists(_folder)) return covers;

        foreach (var file in Directory.EnumerateFiles(_folder).Where(IsSupported).OrderBy(Rank))
            covers.TryAdd(Path.GetFileNameWithoutExtension(file), file);
        return covers;
    }

    // Portada con el mismo nombre; si no hay, una de las disponibles rotando por posición
    public static string? Resolve(IReadOnlyDictionary<string, string> covers, string name, int position)
    {
        if (covers.TryGetValue(name, out var own)) return own;
        return covers.Count == 0 ? null : covers.Values.ElementAt(position % covers.Count);
    }

    private bool IsSupported(string file) => Rank(file) < _extensions.Length;

    private int Rank(string file)
    {
        var index = Array.FindIndex(_extensions, e => e.Equals(Path.GetExtension(file), StringComparison.OrdinalIgnoreCase));
        return index < 0 ? _extensions.Length : index;
    }
}
