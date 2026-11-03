using System.IO;
using System.Windows;
using System.Windows.Media;
using COT_0.Host.ViewModels;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Arma la lista de fondos: degradados del tema + videos de Background:VideosFolder
public class BackgroundCatalog(IConfiguration configuration)
{
    // Recurso de Themes/Backgrounds.xaml con los degradados
    private const string GradientsResourceKey = "BackgroundGradients";

    public IReadOnlyList<BackgroundOption> Load()
    {
        var options = new List<BackgroundOption>();
        var gradientLabel = configuration["Texts:Picker:GradientLabel"] ?? "{0}";
        var videoLabel = configuration["Texts:Picker:VideoLabel"] ?? "{0}";

        foreach (var brush in LoadGradients())
            options.Add(new GradientBackground(FormatLabel(gradientLabel, options.Count), brush));

        foreach (var file in LoadVideoFiles())
            options.Add(new VideoBackground(FormatLabel(videoLabel, options.Count), new Uri(file)));

        return options;
    }

    private static IEnumerable<Brush> LoadGradients() =>
        Application.Current.TryFindResource(GradientsResourceKey) as Brush[] ?? [];

    private IEnumerable<string> LoadVideoFiles()
    {
        var folder = configuration["Background:VideosFolder"];
        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder)) return [];

        var extensions = configuration.GetSection("Background:VideoExtensions").Get<string[]>() ?? [];
        return Directory.EnumerateFiles(folder)
            .Where(file => extensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
            .Order();
    }

    // Numeración del diseño: 01, 02, 03...
    private static string FormatLabel(string format, int index) =>
        string.Format(format, (index + 1).ToString("00"));
}
