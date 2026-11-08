// Canciones de Audio:MusicFolder: lista ordenada y orden de reproducción
using COT_0.Core.Services;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Musica;

/// <summary>La carpeta vive fuera del repo (Paths:MediaRoot); leerla va fuera del hilo de la UI.</summary>
public class MusicLibrary
{
    private static readonly StringComparer Order = StringComparer.OrdinalIgnoreCase;

    private readonly HashSet<string> _extensions = new(AppConfig.Settings.Audio.Extensions, StringComparer.OrdinalIgnoreCase);

    public string Folder { get; } = MediaPath.Resolve(AppConfig.Settings.Paths.MediaRoot, AppConfig.Settings.Audio.MusicFolder);

    public bool FolderExists => !string.IsNullOrWhiteSpace(Folder) && Directory.Exists(Folder);

    public Task<IReadOnlyList<string>> GetTracksAsync() => Task.Run<IReadOnlyList<string>>(() =>
        FolderExists ? Directory.EnumerateFiles(Folder).Where(IsSupported).Order(Order).ToList() : []);

    /// <summary>La siguiente en orden alfabético; al final vuelve a la primera.</summary>
    public static string NextAfter(IReadOnlyList<string> tracks, string current) =>
        tracks.FirstOrDefault(track => Order.Compare(track, current) > 0) ?? tracks[0];

    /// <summary>La anterior en orden alfabético; al principio salta a la última.</summary>
    public static string PreviousBefore(IReadOnlyList<string> tracks, string current) =>
        tracks.LastOrDefault(track => Order.Compare(track, current) < 0) ?? tracks[^1];

    private bool IsSupported(string file) => _extensions.Contains(Path.GetExtension(file));
}
