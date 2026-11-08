// Selector de fondos: miniaturas, elegir uno, cuadro fijo para videos, sin fondo y errores en InfoBar
using COT_0.Core.Models;
using System.Collections.ObjectModel;
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Fondos;
using COT_0.Host.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Pantalla FONDOS: catálogo de la carpeta,
/// y el fondo vivo se ve detrás con el velo oscuro.
/// </summary>
public partial class FondosViewModel : ObservableObject
{
    private readonly FondoService _fondos;
    private readonly FondoImages _images;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isErrorOpen;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _selectedUsesStill;

    [ObservableProperty]
    private bool _canUseStill;

    public FondosViewModel(FondoService fondos, FondoImages images)
    {
        _fondos = fondos;
        _images = images;
        fondos.Changed += Refresh;
    }

    public ObservableCollection<FondoRowViewModel> Items { get; } = [];

    /// <summary>Relee la carpeta cada vez que abres la pantalla (pudiste agregar fondos).</summary>
    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            await _fondos.LoadAsync();
            Items.Clear();
            foreach (var fondo in _fondos.Items) Items.Add(new FondoRowViewModel(fondo));
            Refresh();
            // Una por una: no se decodifican todas a la vez
            foreach (var row in Items.ToList())
                await row.LoadThumbnailAsync(_images, _fondos.PathOf(row.Fondo), AppConfig.Settings.Background.ThumbnailSize);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private Task PickAsync(FondoRowViewModel row) => _fondos.SelectAsync(row.Fondo);

    [RelayCommand]
    private Task ClearAsync() => _fondos.SelectAsync(null);

    [RelayCommand]
    private Task ReloadAsync() => LoadAsync();

    /// <summary>Video elegido: alterna entre reproducirlo y su cuadro fijo (más ligero).</summary>
    [RelayCommand]
    private Task ToggleStillAsync() =>
        _fondos.Selected is { Kind: FondoKind.Video } fondo ? _fondos.SetUseStillAsync(fondo, !fondo.UseStill) : Task.CompletedTask;

    private void Refresh()
    {
        var selected = _fondos.Selected;
        foreach (var row in Items)
            row.IsSelected = selected is not null && row.Fondo.FileName == selected.FileName;

        CanUseStill = selected?.Kind == FondoKind.Video;
        SelectedUsesStill = selected?.UseStill ?? false;
        ShowError(_fondos.Error, _fondos.ErrorDetail, _fondos.ErrorCode);
    }

    // Carpeta que no existe o vacía, archivo ilegible (con la causa y el código de Windows) o fondos.json dañado
    private void ShowError(FondoError error, string? detail, string? code)
    {
        IsErrorOpen = error != FondoError.None;
        if (IsErrorOpen) ErrorMessage = string.Format(UiText.Get($"Text.Backgrounds.Error.{error}"), detail, code);
    }
}
