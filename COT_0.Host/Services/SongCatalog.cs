using System.IO;
using System.Windows.Media;
using COT_0.Host.ViewModels;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Canciones para el selector: título = nombre del archivo, duración leída del archivo
// y portada con el mismo nombre o, si falta, una de las disponibles en ciclo
public class SongCatalog(MusicLibrary library, CoverLibrary covers, CoverDecoder decoder, IConfiguration configuration)
{
    public Task<IReadOnlyList<SongItem>> LoadAsync() => Task.Run(Load);

    private IReadOnlyList<SongItem> Load()
    {
        var durationFormat = configuration["Audio:DurationFormat"] ?? string.Empty;
        var available = covers.GetCovers();
        var decoded = new Dictionary<string, ImageSource?>(StringComparer.OrdinalIgnoreCase);

        return library.GetTracks()
            .Select((track, position) =>
            {
                var name = Path.GetFileNameWithoutExtension(track);
                var cover = CoverLibrary.Resolve(available, name, position);
                return new SongItem(track, name, ReadDuration(track, durationFormat), cover is null ? null : Decoded(cover));
            })
            .ToList();

        // Cada archivo se decodifica una sola vez aunque lo usen varias canciones
        ImageSource? Decoded(string file)
        {
            if (!decoded.TryGetValue(file, out var image)) decoded[file] = image = decoder.Decode(file);
            return image;
        }
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
}
