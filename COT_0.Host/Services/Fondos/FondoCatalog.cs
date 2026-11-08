// Lee la carpeta de fondos (Paths:MediaRoot + Background:Folder): imágenes y videos con su tamaño
using COT_0.Core.Services;
using COT_0.Core.Models;
using COT_0.Host.Services.Configuracion;

namespace COT_0.Host.Services.Fondos;

/// <summary>Async y sin tipos de vista. La app no crea la carpeta.</summary>
public class FondoCatalog : IBackgroundCatalog
{
    private static readonly StringComparer Order = StringComparer.OrdinalIgnoreCase;

    private readonly HashSet<string> _images = new(Settings.ImageExtensions, StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _videos = new(Settings.VideoExtensions, StringComparer.OrdinalIgnoreCase);

    public string Folder { get; } = MediaPath.Resolve(AppConfig.Settings.Paths.MediaRoot, Settings.Folder);

    public bool FolderExists => !string.IsNullOrWhiteSpace(Folder) && Directory.Exists(Folder);

    private static BackgroundSettings Settings => AppConfig.Settings.Background;

    public string PathOf(Fondo fondo) => Path.Combine(Folder, fondo.FileName);

    /// <summary>Los pesados (más de Background:HeavyVideoMegabytes) arrancan como cuadro fijo.</summary>
    public static bool IsHeavy(FondoKind kind, long sizeBytes) =>
        kind == FondoKind.Video && sizeBytes > Settings.HeavyVideoMegabytes * 1024 * 1024;

    /// <summary>Fondos ordenados por nombre; vacío si la carpeta no existe.</summary>
    public Task<IReadOnlyList<Fondo>> ScanAsync() => Task.Run<IReadOnlyList<Fondo>>(() =>
        FolderExists
            ? new DirectoryInfo(Folder).EnumerateFiles().OrderBy(f => f.Name, Order).Select(ToFondo).OfType<Fondo>().ToList()
            : []);

    private Fondo? ToFondo(FileInfo file)
    {
        FondoKind? kind = _images.Contains(file.Extension) ? FondoKind.Image : _videos.Contains(file.Extension) ? FondoKind.Video : null;
        return kind is { } k ? new Fondo(file.Name, k, file.Length, IsHeavy(k, file.Length)) : null;
    }
}
