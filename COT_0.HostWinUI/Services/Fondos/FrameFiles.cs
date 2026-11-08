// Guarda en disco (JPEG) la imagen que entrega Windows para un cuadro de video
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace COT_0.HostWinUI.Services.Fondos;

/// <summary>Lo usan los intentos de Windows (miniatura y MediaComposition); todo async.</summary>
public static class FrameFiles
{
    /// <summary>Recodifica a JPEG (la miniatura de Windows puede venir en BMP o PNG).</summary>
    public static async Task SaveAsJpegAsync(IRandomAccessStream source, string output)
    {
        var decoder = await BitmapDecoder.CreateAsync(source);
        using var bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore);
        await using var file = File.Create(output);
        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, file.AsRandomAccessStream());
        encoder.SetSoftwareBitmap(bitmap);
        await encoder.FlushAsync();
    }

    /// <summary>Copia tal cual (MediaComposition ya entrega JPEG).</summary>
    public static async Task CopyAsync(IRandomAccessStream source, string output)
    {
        await using var file = File.Create(output);
        await source.AsStreamForRead().CopyToAsync(file);
    }
}
