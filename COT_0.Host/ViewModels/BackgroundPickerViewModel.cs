using System.IO;
using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.ViewModels;

// Selector "Cambiar fondo"; el fondo elegido se comparte entre dashboard y sala
public partial class BackgroundPickerViewModel : ObservableObject
{
    [ObservableProperty]
    private BackgroundOption? _selected;

    [ObservableProperty]
    private bool _isOpen;

    public BackgroundPickerViewModel(BackgroundCatalog catalog, IConfiguration configuration)
    {
        Options = catalog.Load();
        // Arranca con el fondo por defecto (por nombre de archivo) hasta que el usuario elija otro
        var defaultFile = configuration["Background:DefaultFile"];
        _selected = Options.FirstOrDefault(option =>
            string.Equals(Path.GetFileName(option.Source.LocalPath), defaultFile, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<BackgroundOption> Options { get; }

    [RelayCommand]
    private void Open() => IsOpen = true;

    [RelayCommand]
    private void Close() => IsOpen = false;

    [RelayCommand]
    private void Pick(BackgroundOption option)
    {
        Selected = option;
        IsOpen = false;
    }
}
