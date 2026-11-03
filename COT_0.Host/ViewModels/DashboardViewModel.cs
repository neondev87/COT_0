using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using COT_0.Core.Services;
using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.ViewModels;

// Dashboard: reloj, estado de la sala y menú (QR, fondo, manual, salir)
public partial class DashboardViewModel : ObservableObject
{
    private readonly WebServerHost _webServer;
    private readonly ITunnelService _tunnel;
    private readonly IRoomService _room;
    private readonly IChatService _chat;
    private readonly IAuthService _auth;
    private readonly ScreenNavigator _screens;
    private readonly WindowNavigator _windows;
    private readonly DialogService _dialogs;
    private readonly IConfiguration _config;
    private readonly CultureInfo _culture;

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
    private string _qrSubtitle;

    public DashboardViewModel(
        WebServerHost webServer, ITunnelService tunnel, IRoomService room, IChatService chat, IAuthService auth,
        ScreenNavigator screens, WindowNavigator windows, DialogService dialogs,
        BackgroundPickerViewModel picker, IConfiguration config)
    {
        _webServer = webServer;
        _tunnel = tunnel;
        _room = room;
        _chat = chat;
        _auth = auth;
        _screens = screens;
        _windows = windows;
        _dialogs = dialogs;
        _config = config;
        _culture = CultureInfo.GetCultureInfo(config["Ui:Culture"] ?? "es-AR");
        _qrSubtitle = Text("Dashboard:QrSubtitle");
        Picker = picker;

        var clock = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        clock.Tick += (_, _) => UpdateClock();
        clock.Start();
        UpdateClock();

        room.Changed += OnRoomStateChanged;
        chat.UsersChanged += OnRoomStateChanged;
    }

    public BackgroundPickerViewModel Picker { get; }

    [RelayCommand]
    private async Task GenerateQrAsync()
    {
        if (_room.IsOpen)
        {
            _screens.ShowRoom();
            return;
        }

        QrSubtitle = Text("Dashboard:QrBusy");
        try
        {
            await _webServer.StartAsync();
            await _tunnel.StartAsync(WebServerHost.Url);
            _room.Open();
            _screens.ShowRoom();
        }
        catch (Exception ex)
        {
            _dialogs.ShowError(Text("Dashboard:QrError"), ex.Message);
        }
        finally
        {
            QrSubtitle = Text("Dashboard:QrSubtitle");
        }
    }

    [RelayCommand]
    private void ShowRoom() => _screens.ShowRoom();

    [RelayCommand]
    private void ShowManual() => _dialogs.ShowManual();

    [RelayCommand]
    private void Logout()
    {
        _auth.Logout();
        Picker.CloseCommand.Execute(null);
        _screens.ShowDashboard();
        _windows.ShowLogin();
    }

    private void UpdateClock()
    {
        var now = DateTime.Now;
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
