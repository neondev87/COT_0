using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using COT_0.Core.Services;
using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.ViewModels;

// Sala activa: QR, URL, contador, lista de conectados y emotes
public partial class RoomViewModel : ObservableObject
{
    private readonly IRoomService _room;
    private readonly IChatService _chat;
    private readonly RoomSession _session;
    private readonly QrService _qr;
    private readonly ScreenNavigator _screens;
    private readonly ClockService _clock;
    private readonly IConfiguration _config;

    [ObservableProperty]
    private string _code = string.Empty;

    [ObservableProperty]
    private string _elapsed = string.Empty;

    [ObservableProperty]
    private string _joinUrl = string.Empty;

    [ObservableProperty]
    private ImageSource? _qrImage;

    [ObservableProperty]
    private bool _isCopied;

    [ObservableProperty]
    private int _total;

    public RoomViewModel(IRoomService room, IChatService chat, RoomSession session, QrService qr,
        ScreenNavigator screens, ClockService clock, IConfiguration config, EmotesViewModel emotes)
    {
        Emotes = emotes;
        _room = room;
        _chat = chat;
        _session = session;
        _qr = qr;
        _screens = screens;
        _clock = clock;
        _config = config;

        clock.Tick += Tick;

        room.Changed += () => UiThread.Run(OnRoomChanged);
        chat.UsersChanged += () => UiThread.Run(() => _ = ReloadUsersAsync());
    }

    public ObservableCollection<UserRowViewModel> Users { get; } = [];

    public EmotesViewModel Emotes { get; }

    [RelayCommand]
    private void Back() => _screens.ShowDashboard();

    [RelayCommand]
    private async Task CopyAsync()
    {
        try
        {
            Clipboard.SetText(JoinUrl);
        }
        catch (COMException)
        {
            // Portapapeles ocupado por otra app
            return;
        }

        IsCopied = true;
        await Task.Delay(_config.GetValue("Ui:CopyFeedbackMilliseconds", 1600));
        IsCopied = false;
    }

    [RelayCommand]
    private Task KickAsync(UserRowViewModel user) => _chat.KickUserAsync(user.Id);

    [RelayCommand]
    private async Task CloseRoomAsync()
    {
        await _session.CloseAsync();
        _screens.ShowDashboard();
    }

    private void OnRoomChanged()
    {
        Code = _room.Code ?? string.Empty;
        if (!_room.IsOpen)
        {
            Elapsed = string.Empty;
            JoinUrl = string.Empty;
            QrImage = null;
            return;
        }

        JoinUrl = _session.Url + _config["Web:JoinPath"];
        QrImage = _qr.Generate(JoinUrl);
        Tick();
        _ = ReloadUsersAsync();
    }

    private async Task ReloadUsersAsync()
    {
        var sinceFormat = _config["Texts:Room:Since"] ?? "{0}";
        var users = await _chat.GetUsersAsync();

        Users.Clear();
        foreach (var user in users.OrderBy(u => u.JoinedAt))
            Users.Add(new UserRowViewModel(user, sinceFormat, _clock));
        Total = Users.Count;
    }

    private void Tick()
    {
        if (_room.OpenedAt is not { } openedAt) return;

        // Horas totales: no se reinicia a las 24 h
        var elapsed = _clock.Since(openedAt);
        Elapsed = string.Format(_config["Ui:ElapsedFormat"] ?? "{0}", (int)elapsed.TotalHours, elapsed.Minutes, elapsed.Seconds);
        foreach (var user in Users) user.Refresh();
    }
}
