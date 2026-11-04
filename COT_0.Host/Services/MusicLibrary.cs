using System.IO;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Canciones de Audio:MusicFolder: lista ordenada, orden de reproducción e importación de archivos
public class MusicLibrary
{
    private static readonly StringComparer Order = StringComparer.OrdinalIgnoreCase;

    private readonly string _folder;
    private readonly HashSet<string> _extensions;

    public MusicLibrary(IConfiguration configuration)
    {
        _folder = configuration["Audio:MusicFolder"] ?? string.Empty;
        _extensions = new(configuration.GetSection("Audio:Extensions").Get<string[]>() ?? [], StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<string> Extensions => _extensions;

    public void EnsureFolder()
    {
        if (!string.IsNullOrWhiteSpace(_folder)) Directory.CreateDirectory(_folder);
    }

    public IReadOnlyList<string> GetTracks()
    {
        if (string.IsNullOrWhiteSpace(_folder) || !Directory.Exists(_folder)) return [];
        return Directory.EnumerateFiles(_folder).Where(IsSupported).Order(Order).ToList();
    }

    // La siguiente en orden alfabético; al final vuelve a la primera
    public static string NextAfter(IReadOnlyList<string> tracks, string current) =>
        tracks.FirstOrDefault(track => Order.Compare(track, current) > 0) ?? tracks[0];

    // Copia las canciones a la carpeta; las que ya existen con ese nombre se saltean
    public async Task ImportAsync(IEnumerable<string> files)
    {
        if (string.IsNullOrWhiteSpace(_folder)) return;
        EnsureFolder();
        foreach (var file in files.Where(IsSupported))
        {
            var target = Path.Combine(_folder, Path.GetFileName(file));
            if (File.Exists(target)) continue;
            await using var source = File.OpenRead(file);
            await using var destination = new FileStream(target, FileMode.CreateNew);
            await source.CopyToAsync(destination);
        }
    }

    private bool IsSupported(string file) => _extensions.Contains(Path.GetExtension(file));
}
