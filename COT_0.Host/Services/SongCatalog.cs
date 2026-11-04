using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using COT_0.Host.ViewModels;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Canciones para el selector: título = nombre del archivo, duración leída del archivo
// y portada de Audio:CoversFolder con el mismo nombre
public class SongCatalog(MusicLibrary library, IConfiguration configuration)
{
    public Task<IReadOnlyList<SongItem>> LoadAsync() => Task.Run(Load);

    private IReadOnlyList<SongItem> Load()
    {
        var durationFormat = configuration["Audio:DurationFormat"] ?? string.Empty;
        return library.GetTracks()
            .Select(track => new SongItem(track, Path.GetFileNameWithoutExtension(track),
                ReadDuration(track, durationFormat), LoadCover(Path.GetFileNameWithoutExtension(track))))
            .ToList();
    }

    private static string ReadDuration(string track, string format)
    {
        try
        {
            using var file = TagLib.File.Create(track);
            return file.Properties.Duration.ToString(format);
        }
        catch (Exception ex) when (ex is TagLib.CorruptFileException or TagLib.UnsupportedFormatException or IOException)
        {
            return string.Empty;
        }
    }

    private ImageSource? LoadCover(string name)
    {
        var folder = configuration["Audio:CoversFolder"];
        if (string.IsNullOrWhiteSpace(folder)) return null;

        var extensions = configuration.GetSection("Audio:CoverExtensions").Get<string[]>() ?? [];
        var file = extensions.Select(extension => Path.Combine(folder, name + extension)).FirstOrDefault(File.Exists);
        return file is null ? null : Decode(file);
    }

    // Decodifica chica y congelada para poder crearla fuera del hilo de la UI
    private BitmapImage? Decode(string file)
    {
        try
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.DecodePixelHeight = configuration.GetValue<int>("Audio:CoverDecodeSize");
            image.UriSource = new Uri(file);
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch (Exception ex) when (ex is NotSupportedException or IOException or FileFormatException)
        {
            return null;
        }
    }
}
