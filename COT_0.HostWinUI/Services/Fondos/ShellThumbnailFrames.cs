// Segundo intento del cuadro fijo: la miniatura que genera Windows (la del explorador)
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Registro;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace COT_0.HostWinUI.Services.Fondos;

/// <summary>
/// Con ThumbnailMode.SingleItem Windows regresaba el icono del tipo de archivo (ThumbnailType.Icon) para estos
/// videos: por eso la cuadrícula salía vacía. VideosView sí extrae un cuadro; si aun así llega un icono, se descarta.
/// </summary>
public class ShellThumbnailFrames
{
    public async Task<bool> TryExtractAsync(string video, string output)
    {
        try
        {
            var file = await StorageFile.GetFileFromPathAsync(video);
            using var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.VideosView, AppConfig.Settings.Background.FrameMaxWidth,
                ThumbnailOptions.ResizeThumbnail);
            if (thumbnail is null || thumbnail.Type != ThumbnailType.Image)
            {
                var got = thumbnail?.Type.ToString() ?? "nada";
                await MediaLog.WriteAsync("miniatura-windows", video, null, $"Windows dio {got} en lugar de un cuadro");
                return false;
            }
            await FrameFiles.SaveAsJpegAsync(thumbnail, output);
            return true;
        }
        catch (Exception ex) when (FondoFailures.IsMediaException(ex))
        {
            await MediaLog.WriteAsync("miniatura-windows", video, ex);
            return false;
        }
    }
}
