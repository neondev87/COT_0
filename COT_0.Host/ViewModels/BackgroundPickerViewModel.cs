using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.ViewModels;

// Selector "Cambiar fondo"; el fondo elegido se comparte entre dashboard y sala
public partial class BackgroundPickerViewModel(BackgroundCatalog catalog) : ObservableObject
{
    [ObservableProperty]
    private BackgroundOption? _selected;

    [ObservableProperty]
    private bool _isOpen;

    public IReadOnlyList<BackgroundOption> Options { get; } = catalog.Load();

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
