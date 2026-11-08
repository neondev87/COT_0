// Tercer intento del cuadro fijo: Media Foundation decodifica el cuadro (Windows.Media.Editing)
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Registro;
using Windows.Media.Editing;
using Windows.Storage;

namespace COT_0.HostWinUI.Services.Fondos;

/// <summary>
/// En lugar del "frame server" de MediaPlayer (que necesita una superficie Direct3D, o sea Win2D) se usa
/// MediaComposition.GetThumbnailAsync: abre el video con los mismos decodificadores y entrega el cuadro en JPEG.
/// </summary>
public class CompositionFrames
{
    public async Task<bool> TryExtractAsync(string video, string output)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(video);
            var composition = new MediaComposition();
            composition.Clips.Add(await MediaClip.CreateFromFileAsync(file));
            var settings = AppConfig.Settings.Background;
            var at = TimeSpan.FromSeconds(Math.Min(settings.FrameSeconds, composition.Duration.TotalSeconds / 2));
            using var frame = await composition.GetThumbnailAsync(at, (int)settings.FrameMaxWidth, 0, VideoFramePrecision.NearestKeyFrame);
            await FrameFiles.CopyAsync(frame, output);
            return true;
        }
        catch (Exception ex) when (FondoFailures.IsMediaException(ex))
        {
            await MediaLog.WriteAsync("cuadro-mediacomposition", video, ex);
            return false;
        }
    }
}
