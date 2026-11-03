using System.IO;
using COT_0.Host.ViewModels;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Arma la lista de fondos con las imágenes y videos de Background:Folder
public class BackgroundCatalog(IConfiguration configuration)
{
    public IReadOnlyList<BackgroundOption> Load()
    {
        var folder = configuration["Background:Folder"];
        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder)) return [];

        var images = ReadExtensions("Background:ImageExtensions");
        var videos = ReadExtensions("Background:VideoExtensions");
        var preview = TimeSpan.FromSeconds(configuration.GetValue<double>("Background:PreviewSeconds"));

        return Directory.EnumerateFiles(folder)
            .Order()
            .Select(file => ToOption(file, images, videos, preview))
            .OfType<BackgroundOption>()
            .ToList();
    }

    private static BackgroundOption? ToOption(string file, ISet<string> images, ISet<string> videos, TimeSpan preview)
    {
        var extension = Path.GetExtension(file);
        if (images.Contains(extension)) return new ImageBackground(new Uri(file));
        if (videos.Contains(extension)) return new VideoBackground(new Uri(file), preview);
        return null;
    }

    private HashSet<string> ReadExtensions(string key) =>
        new(configuration.GetSection(key).Get<string[]>() ?? [], StringComparer.OrdinalIgnoreCase);
}
