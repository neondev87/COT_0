// Portadas de Audio:CoversFolder: una por nombre de canción (la primera extensión de Audio:CoverExtensions gana)
using COT_0.Core.Services;
using COT_0.HostWinUI.Services.Configuracion;

namespace COT_0.HostWinUI.Services.Musica;

/// <summary>Igual que el CoverLibrary del WPF; si una canción no tiene portada propia, se presta una en ciclo.</summary>
public class CoverLibrary
{
    private readonly IReadOnlyList<string> _extensions = AppConfig.Settings.Audio.CoverExtensions;

    private readonly string _folder = MediaPath.Resolve(AppConfig.Settings.Paths.MediaRoot, AppConfig.Settings.Audio.CoversFolder);

    /// <summary>Archivo de portada por nombre sin extensión, ordenado por nombre.</summary>
    public Task<IReadOnlyDictionary<string, string>> GetCoversAsync() => Task.Run<IReadOnlyDictionary<string, string>>(() =>
    {
        var covers = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(_folder) || !Directory.Exists(_folder)) return covers;

        foreach (var file in Directory.EnumerateFiles(_folder).Where(IsSupported).OrderBy(Rank))
            covers.TryAdd(Path.GetFileNameWithoutExtension(file), file);
        return covers;
    });

    /// <summary>Portada con el mismo nombre; si no hay, una de las disponibles rotando por posición.</summary>
    public static string? Resolve(IReadOnlyDictionary<string, string> covers, string name, int position)
    {
        if (covers.TryGetValue(name, out var own)) return own;
        return covers.Count == 0 ? null : covers.Values.ElementAt(position % covers.Count);
    }

    private bool IsSupported(string file) => Rank(file) < _extensions.Count;

    private int Rank(string file)
    {
        var extension = Path.GetExtension(file);
        for (var i = 0; i < _extensions.Count; i++)
            if (_extensions[i].Equals(extension, StringComparison.OrdinalIgnoreCase)) return i;
        return _extensions.Count;
    }
}
