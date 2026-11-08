using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Imagenes;

/// <summary>Implementa IImageUploader: el navegador achica la imagen, se ve la vista previa y se sube al enviar (Scoped).</summary>
public sealed class ImageUploader(IImageService images, IOptions<ChatOptions> options, ILogger<ImageUploader> logger) : IImageUploader
{
    private readonly ChatOptions _options = options.Value;
    private IBrowserFile? _resized;

    public ImageUploadState State { get; private set; }
    public string? PreviewUrl { get; private set; }
    public string? Notice { get; private set; }
    public int InputVersion { get; private set; }

    public async Task SelectAsync(IBrowserFile file)
    {
        if (State != ImageUploadState.Idle) return;
        State = ImageUploadState.Preparing;
        Notice = null;

        try
        {
            // El navegador la decodifica (también HEIC en iPhone), la achica y la pasa a JPEG: no se sube el original
            _resized = await ResizeAsync(file, _options.ImageMaxSide);
            if (_resized.Size > _options.MaxImageBytes)
            {
                Reset(ImageNotice.ForSize(_options.MaxImageBytes));
                return;
            }

            var preview = await ResizeAsync(file, _options.ImagePreviewSide);
            PreviewUrl = $"data:{preview.ContentType};base64,{Convert.ToBase64String(await ReadAsync(preview))}";
            State = ImageUploadState.Preview;
        }
        catch (Exception ex)
        {
            // Formato que el navegador no sabe abrir, se cortó la conexión o tardó demasiado
            logger.LogWarning(ex, "No se pudo preparar la imagen");
            Reset(ImageNotice.Unreadable);
        }
    }

    public async Task SendAsync(string userId)
    {
        if (State != ImageUploadState.Preview || _resized is null) return;
        State = ImageUploadState.Uploading;
        string? notice;

        try
        {
            await using var stream = _resized.OpenReadStream(_options.MaxImageBytes);
            notice = (await images.SendAsync(userId, stream)).Error;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "No se pudo subir la imagen");
            notice = ImageNotice.SendFailed;
        }

        Reset(notice);
    }

    public void Cancel() => Reset(null);

    public void DismissNotice() => Notice = null;

    // Blazor no rechaza si el navegador no puede abrir la imagen: se corta con Chat:ImageDecodeTimeoutSeconds
    private async Task<IBrowserFile> ResizeAsync(IBrowserFile file, int maxSide) =>
        await file.RequestImageFileAsync(_options.ImageFormat, maxSide, maxSide).AsTask()
            .WaitAsync(TimeSpan.FromSeconds(_options.ImageDecodeTimeoutSeconds));

    private async Task<byte[]> ReadAsync(IBrowserFile file)
    {
        using var buffer = new MemoryStream();
        await using var input = file.OpenReadStream(_options.MaxImageBytes);
        await input.CopyToAsync(buffer);
        return buffer.ToArray();
    }

    private void Reset(string? notice)
    {
        _resized = null;
        PreviewUrl = null;
        Notice = notice;
        State = ImageUploadState.Idle;
        InputVersion++;
    }
}
