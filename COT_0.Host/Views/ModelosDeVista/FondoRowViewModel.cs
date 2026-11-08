// Miniatura del selector de fondos: nombre, tipo y tamaño, si es pesado y si es el elegido
using System.Globalization;
using COT_0.Host.Models;
using COT_0.Host.Services.Fondos;
using COT_0.Host.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Se crea en el hilo de la UI; la miniatura llega después (LoadThumbnailAsync).</summary>
public partial class FondoRowViewModel(Fondo fondo) : ObservableObject
{
    private const double BytesPerMegabyte = 1024 * 1024;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private ImageSource? _thumbnail;

    public Fondo Fondo { get; } = fondo;

    /// <summary>Sin el ID de YouTube ni signos de ancho completo (FondoNames).</summary>
    public string Name { get; } = FondoNames.Clean(fondo.FileName);

    public string Detail { get; } = string.Format(CultureInfo.CurrentCulture, UiText.Get("Text.Backgrounds.Detail"),
        UiText.Get($"Text.Backgrounds.Kind.{fondo.Kind}"), fondo.SizeBytes / BytesPerMegabyte);

    public bool IsHeavy { get; } = FondoCatalog.IsHeavy(fondo.Kind, fondo.SizeBytes);

    public async Task LoadThumbnailAsync(FondoImages images, string path, uint size) =>
        Thumbnail = await images.LoadAsync(path, Fondo.Kind, size);
}
