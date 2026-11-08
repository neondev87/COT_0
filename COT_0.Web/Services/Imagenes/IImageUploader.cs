using Microsoft.AspNetCore.Components.Forms;

namespace COT_0.Web.Services.Imagenes;

// Adjunto de imagen del invitado (Scoped, uno por circuito): el navegador la achica, se ve la vista previa y se sube al enviar
public interface IImageUploader
{
    ImageUploadState State { get; }
    string? PreviewUrl { get; }
    string? Notice { get; }

    // Cambia al terminar: el <InputFile> se recrea y se puede volver a elegir la misma foto
    int InputVersion { get; }

    Task SelectAsync(IBrowserFile file);
    Task SendAsync(string userId);
    void Cancel();
    void DismissNotice();
}
