using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.ViewModels;

// Ventana principal: pantalla actual (dashboard o sala), fondo compartido y selector de canciones
public partial class MainViewModel : ObservableObject
{
    private readonly ScreenNavigator _screens;
    private readonly DashboardViewModel _dashboard;
    private readonly RoomViewModel _room;

    [ObservableProperty]
    private ObservableObject _current;

    public MainViewModel(ScreenNavigator screens, DashboardViewModel dashboard, RoomViewModel room,
        BackgroundPickerViewModel picker, SongPickerViewModel songs)
    {
        _screens = screens;
        _dashboard = dashboard;
        _room = room;
        Picker = picker;
        Songs = songs;
        _current = Resolve();
        screens.Changed += () => Current = Resolve();
    }

    public BackgroundPickerViewModel Picker { get; }

    public SongPickerViewModel Songs { get; }

    private ObservableObject Resolve() => _screens.Current == Screen.Room ? _room : _dashboard;
}
