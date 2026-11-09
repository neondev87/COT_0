// Abre el selector de Windows para elegir la foto de perfil (png o jpg)
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Ventana;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace COT_0.Host.Services.Perfiles;

/// <summary>FileOpenPicker en una app sin empaquetar necesita el HWND de la ventana (WindowSetup.Handle).</summary>
public static class PhotoPicker
{
    /// <summary>null si cancelaste.</summary>
    public static async Task<StorageFile?> PickAsync()
    {
        var picker = new FileOpenPicker { ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.PicturesLibrary };
        foreach (var extension in AppConfig.Settings.Profiles.PhotoExtensions) picker.FileTypeFilter.Add(extension);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, WindowSetup.Handle);
        return await picker.PickSingleFileAsync();
    }
}
