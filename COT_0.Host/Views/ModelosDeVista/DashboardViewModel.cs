// Dashboard: reloj, estado de la sala y menú (QR / sala, fondo, manual, usuarios, tienda de emotes, música, salir)
using System.Globalization;
using COT_0.Core.Services;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Sala;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Tiempo;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Ventana;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Fondos, emotes y música son su propia pantalla.
/// IsRoomActive / IsRoomInactive reemplazan al DataTrigger del estado.
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    private readonly RoomSession _session;
    private readonly IRoomService _room;
    private readonly IChatService _chat;
    private readonly ClockService _clock;
    private readonly CultureInfo _culture;

    [ObservableProperty]
    private string _time = string.Empty;

    [ObservableProperty]
    private string _seconds = string.Empty;

    [ObservableProperty]
    private string _date = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRoomInactive), nameof(QrTitle), nameof(QrSubtitle))]
    private bool _isRoomActive;

    [ObservableProperty]
    private string _connectedText = string.Empty;

    public DashboardViewModel(RoomSession session, IRoomService room, IChatService chat, ClockService clock)
    {
        _session = session;
        _room = room;
        _chat = chat;
        _clock = clock;
        _culture = CultureInfo.GetCultureInfo(Ui.Culture);

        clock.Tick += UpdateClock;
        room.Changed += OnRoomStateChanged;
        chat.UsersChanged += OnRoomStateChanged;
        UpdateClock();
        OnRoomStateChanged();
        ActiveUser.Changed += () => OnPropertyChanged(nameof(StoreSubtitle));
    }

    public bool IsRoomInactive => !IsRoomActive;

    // Tarjeta 01: GENERAR QR o SALA según haya sala activa
    public string QrTitle => UiText.Get(IsRoomActive ? "Text.Dashboard.RoomTitle" : "Text.Dashboard.QrTitle");

    public string QrSubtitle => UiText.Get(IsRoomActive ? "Text.Dashboard.RoomSubtitle" : "Text.Dashboard.QrSubtitle");

    /// <summary>Tarjeta 05: tienda en modo HOST, punto de venta en modo ADMIN.</summary>
    public string StoreSubtitle => UiText.Get(ActiveUser.IsAdmin ? "Text.Dashboard.StoreSubtitle.Admin" : "Text.Dashboard.StoreSubtitle.Host");

    private static UiSettings Ui => AppConfig.Settings.Ui;

    // Tarjeta 01 y SALA ACTIVA: SalaPage abre la sala (túnel + QR) al cargar
    [RelayCommand]
    private void ShowRoom() => Navigator.ShowRoom();

    [RelayCommand]
    private void ShowBackgrounds() => Navigator.ShowBackgrounds();

    [RelayCommand]
    private void ShowManual() => Navigator.ShowManual();

    [RelayCommand]
    private void ShowUsers() => Navigator.ShowUsers();

    [RelayCommand]
    private void ShowEmotes() => Navigator.ShowEmotes();

    [RelayCommand]
    private void ShowMusic() => Navigator.ShowMusic();

    /// <summary>SALIR: con sala activa pregunta primero; luego cierra sala y túnel y vuelve al login.</summary>
    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (!await _session.ConfirmLogoutAsync()) return;
        if (_session.IsActive) await _session.CloseAsync();
        Navigator.ShowLogin();
    }

    private void UpdateClock()
    {
        var now = _clock.Now;
        Time = now.ToString(Ui.ClockFormat, _culture);
        Seconds = now.ToString(Ui.SecondsFormat, _culture);
        Date = now.ToString(Ui.DateFormat, _culture).ToUpper(_culture);
    }

    // Los eventos llegan desde hilos de la web
    private void OnRoomStateChanged() => UiThread.Run(() => _ = RefreshRoomStateAsync());

    private async Task RefreshRoomStateAsync()
    {
        IsRoomActive = _room.IsOpen;
        var users = await _chat.GetUsersAsync();
        ConnectedText = string.Format(_culture, UiText.Get("Text.Dashboard.Connected"), users.Count);
    }
}
