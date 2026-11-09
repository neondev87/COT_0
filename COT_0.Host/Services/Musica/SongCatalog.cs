// Biblioteca para MusicaPage: título, duración y portada de cada canción (todo async)
using System.Collections.Concurrent;
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using Windows.Storage;

namespace COT_0.Host.Services.Musica;

/// <summary>
/// La duración se lee con las propiedades de música de Windows (sin TagLib) y queda en memoria
/// mientras el archivo no cambie. Las portadas solo se resuelven a ruta: la imagen la decodifica la vista.
/// </summary>
public class SongCatalog(MusicLibrary library, CoverLibrary covers)
{
    private readonly ConcurrentDictionary<(string File, DateTime Modified), string> _durations = new();

    public async Task<IReadOnlyList<Song>> LoadAsync()
    {
        var tracks = await library.GetTracksAsync();
        var available = await covers.GetCoversAsync();

        var songs = new List<Song>(tracks.Count);
        foreach (var track in tracks)
        {
            var title = Path.GetFileNameWithoutExtension(track);
            var cover = CoverLibrary.Resolve(available, title, songs.Count);
            songs.Add(new Song(track, title, await GetDurationAsync(track), cover));
        }
        return songs;
    }

    private async Task<string> GetDurationAsync(string track)
    {
        var key = (track, await Task.Run(() => File.GetLastWriteTimeUtc(track)));
        if (_durations.TryGetValue(key, out var cached)) return cached;

        var duration = await ReadDurationAsync(track);
        _durations[key] = duration;
        return duration;
    }

    // Archivo ilegible o sin metadatos: sin duración, pero la canción sigue en la lista
    private static async Task<string> ReadDurationAsync(string track)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(track);
            var properties = await file.Properties.GetMusicPropertiesAsync();
            return properties.Duration.ToString(AppConfig.Settings.Audio.DurationFormat);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or FormatException
                                       or System.Runtime.InteropServices.COMException)
        {
            return string.Empty;
        }
    }
}
