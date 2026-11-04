using System.Globalization;
using System.Windows;
using COT_0.Core.Services;
using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.ViewModels;

// Dashboard: reloj, estado de la sala y menú (QR, fondo, manual, salir)
public partial class DashboardViewModel : ObservableObject
{
    private readonly RoomSession _session;
    private readonly IRoomService _room;
    private readonly IChatService _chat;
    private readonly IAuthService _auth;
    private readonly ScreenNavigator _screens;
    private readonly WindowNavigator _windows;
    private readonly IShutdownCoordinator _shutdown;
    private readonly DialogService _dialogs;
    private readonly ClockService _clock;
    private readonly IConfiguration _config;
    private readonly CultureInfo _culture;
    private bool _isOpening;

    [ObservableProperty]
    private string _time = string.Empty;

    [ObservableProperty]
    private string _seconds = string.Empty;

    [ObservableProperty]
    private string _date = string.Empty;

    [ObservableProperty]
    private bool _isRoomActive;

    [ObservableProperty]
    private string _connectedText = string.Empty;

    [ObservableProperty]
    private string _qrTitle = string.Empty;

    [ObservableProperty]
    private string _qrSubtitle = string.Empty;

    public DashboardViewModel(
        RoomSession session, IRoomService room, IChatService chat, IAuthService auth,
        ScreenNavigator screens, WindowNavigator windows, IShutdownCoordinator shutdown, DialogService dialogs,
        BackgroundPickerViewModel picker, SignatureViewModel signature, ClockService clock, IConfiguration config)
    {
        _session = session;
        _room = room;
        _chat = chat;
        _auth = auth;
        _screens = screens;
        _windows = windows;
        _shutdown = shutdown;
        _dialogs = dialogs;
        _config = config;
        _culture = CultureInfo.GetCultureInfo(config["Ui:Culture"] ?? "es-AR");
        Picker = picker;
        Signature = signature;
        UpdateQrCard();

        _clock = clock;
        clock.Tick += UpdateClock;
        UpdateClock();

        room.Changed += OnRoomStateChanged;
        chat.UsersChanged += OnRoomStateChanged;
    }

    public BackgroundPickerViewModel Picker { get; }

    public SignatureViewModel Signature { get; }

    // Sin sala crea una; con sala activa vuelve a la existente
    [RelayCommand]
    private async Task GenerateQrAsync()
    {
        if (_room.IsOpen)
        {
            _screens.ShowRoom();
            return;
        }
        if (_isOpening) return;

        _isOpening = true;
        UpdateQrCard();
        try
        {
            await _session.OpenAsync();
            _screens.ShowRoom();
        }
        catch (Exception ex)
        {
            _dialogs.ShowError(Text("Dashboard:QrError"), ex.Message);
        }
        finally
        {
            _isOpening = false;
            UpdateQrCard();
        }
    }

    [RelayCommand]
    private void ShowRoom() => _screens.ShowRoom();

    [RelayCommand]
    private void ShowManual() => _dialogs.ShowManual();

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (!await _shutdown.LeaveRoomAsync()) return;

        _auth.Logout();
        Picker.CloseCommand.Execute(null);
        _screens.ShowDashboard();
        _windows.ShowLogin();
    }

    partial void OnIsRoomActiveChanged(bool value) => UpdateQrCard();

    // Tarjeta 01: GENERAR QR o SALA según haya sala activa
    private void UpdateQrCard()
    {
        QrTitle = Text(IsRoomActive ? "Dashboard:RoomTitle" : "Dashboard:QrTitle");
        QrSubtitle = Text(_isOpening ? "Dashboard:QrBusy" : IsRoomActive ? "Dashboard:RoomSubtitle" : "Dashboard:QrSubtitle");
    }

    private void UpdateClock()
    {
        var now = _clock.Now;
        Time = now.ToString(_config["Ui:ClockFormat"], _culture);
        Seconds = now.ToString(_config["Ui:SecondsFormat"], _culture);
        Date = now.ToString(_config["Ui:DateFormat"], _culture).ToUpper(_culture);
    }

    // Los eventos llegan desde hilos de la web
    private void OnRoomStateChanged() =>
        Application.Current.Dispatcher.Invoke(() => _ = RefreshRoomStateAsync());

    private async Task RefreshRoomStateAsync()
    {
        IsRoomActive = _room.IsOpen;
        var users = await _chat.GetUsersAsync();
        ConnectedText = string.Format(Text("Dashboard:Connected"), users.Count);
    }

    private string Text(string key) => _config["Texts:" + key] ?? string.Empty;
}
