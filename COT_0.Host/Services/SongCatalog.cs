using System.Collections.Concurrent;
using System.IO;
using System.Windows.Media;
using COT_0.Host.ViewModels;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Canciones para el selector: título = nombre del archivo, duración leída del archivo
// y portada con el mismo nombre o, si falta, una de las disponibles en ciclo.
// Portadas y duraciones quedan en memoria entre aperturas; se releen si el archivo cambió
public class SongCatalog(MusicLibrary library, CoverLibrary covers, CoverDecoder decoder, IConfiguration configuration)
{
    private readonly ConcurrentDictionary<(string File, DateTime Modified), ImageSource?> _covers = new();
    private readonly ConcurrentDictionary<(string File, DateTime Modified), string> _durations = new();

    public Task<IReadOnlyList<SongItem>> LoadAsync() => Task.Run(Load);

    private IReadOnlyList<SongItem> Load()
    {
        var durationFormat = configuration["Audio:DurationFormat"] ?? string.Empty;
        var available = covers.GetCovers();

        return library.GetTracks()
            .Select((track, position) =>
            {
                var name = Path.GetFileNameWithoutExtension(track);
                var cover = CoverLibrary.Resolve(available, name, position);
                var duration = _durations.GetOrAdd(Key(track), key => ReadDuration(key.File, durationFormat));
                return new SongItem(track, name, duration, cover is null ? null : _covers.GetOrAdd(Key(cover), key => decoder.Decode(key.File)));
            })
            .ToList();
    }

    private static (string, DateTime) Key(string file) => (file, File.GetLastWriteTimeUtc(file));

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
