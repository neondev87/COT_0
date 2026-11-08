// Sala: abre servidor + túnel + sala, muestra QR, enlace, código y tiempo, y la cierra
using System.Runtime.InteropServices;
using COT_0.Core.Services;
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Sala;
using COT_0.HostWinUI.Services.Textos;
using COT_0.HostWinUI.Services.Tiempo;
using COT_0.HostWinUI.Services.Ventana;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>
/// La parte de RoomViewModel (WPF) que no es la lista de conectados (esa es ConnectedUsersViewModel).
/// En el WPF la sala se abría desde el dashboard; aquí la abre la propia pantalla al entrar (EnsureOpenAsync).
/// IsOpening / IsErrorOpen / IsRoomOpen reemplazan a los DataTrigger.
/// </summary>
public partial class SalaViewModel : ObservableObject
{
    private readonly RoomSession _session;
    private readonly IRoomService _room;
    private readonly QrService _qr;
    private readonly ClockService _clock;

    [ObservableProperty]
    private string _code = string.Empty;

    [ObservableProperty]
    private string _elapsed = string.Empty;

    [ObservableProperty]
    private string _joinUrl = string.Empty;

    [ObservableProperty]
    private BitmapImage? _qrImage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CopyLabel))]
    private bool _isCopied;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStatusVisible))]
    private bool _isRoomOpen;

    [ObservableProperty]
    private bool _isOpening;

    [ObservableProperty]
    private bool _isClosing;

    [ObservableProperty]
    private bool _isErrorOpen;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public SalaViewModel(RoomSession session, IRoomService room, QrService qr, ClockService clock, ConnectedUsersViewModel users)
    {
        _session = session;
        _room = room;
        _qr = qr;
        _clock = clock;
        Users = users;
        users.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ConnectedUsersViewModel.Total)) OnPropertyChanged(nameof(TotalText));
        };

        clock.Tick += Tick;
        room.Changed += () => UiThread.Run(() => _ = RefreshAsync());
        _ = RefreshAsync();
    }

    // Mismo ViewModel que ListaConectados: de ahí sale el total
    private ConnectedUsersViewModel Users { get; }

    /// <summary>"01", "02"…: contador grande junto a "Personas conectadas".</summary>
    public string TotalText => Users.Total.ToString("00", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Mientras no hay sala se ven los avisos (abriendo o error) en lugar del QR.</summary>
    public bool IsStatusVisible => !IsRoomOpen;

    public string CopyLabel => UiText.Get(IsCopied ? "Text.Room.Copied" : "Text.Room.Copy");

    /// <summary>Lo llama SalaPage al entrar: si la sala no está abierta, la abre.</summary>
    public Task EnsureOpenAsync() => _room.IsOpen ? Task.CompletedTask : OpenAsync();

    /// <summary>GENERAR QR (al entrar) y REINTENTAR del aviso de error.</summary>
    [RelayCommand]
    private async Task OpenAsync()
    {
        if (IsOpening || _room.IsOpen) return;

        IsOpening = true;
        IsErrorOpen = false;
        try
        {
            await _session.OpenAsync();
        }
        catch (Exception ex) when (RoomOpenErrors.IsOpenError(ex))
        {
            var error = RoomOpenErrors.Classify(ex, _session.IsServerRunning);
            ErrorMessage = RoomOpenErrors.Message(error, ex);
            IsErrorOpen = true;
        }
        finally
        {
            IsOpening = false;
        }
    }

    /// <summary>CERRAR SALA: saca a todos, cierra la sesión y el túnel, y regresa al dashboard.</summary>
    [RelayCommand]
    private async Task CloseRoomAsync()
    {
        if (IsClosing) return;

        IsClosing = true;
        try
        {
            await _session.CloseAsync();
        }
        finally
        {
            IsClosing = false;
        }
        Navigator.ShowDashboard();
    }

    [RelayCommand]
    private async Task CopyAsync()
    {
        try
        {
            var package = new DataPackage();
            package.SetText(JoinUrl);
            Clipboard.SetContent(package);
        }
        catch (COMException)
        {
            // Portapapeles ocupado por otra app
            return;
        }

        IsCopied = true;
        await Task.Delay(AppConfig.Settings.Ui.CopyFeedbackMilliseconds);
        IsCopied = false;
    }

    [RelayCommand]
    private void ShowEmotes() => Navigator.ShowEmotes();

    [RelayCommand]
    private void ShowMusic() => Navigator.ShowMusic();

    // Sala abierta → QR del enlace público; cerrada → se limpia todo
    private async Task RefreshAsync()
    {
        IsRoomOpen = _room.IsOpen;
        Code = _room.Code is { } code ? $"#{code}" : string.Empty;
        if (!IsRoomOpen)
        {
            Elapsed = string.Empty;
            JoinUrl = string.Empty;
            QrImage = null;
            return;
        }

        JoinUrl = _session.Url + AppConfig.Settings.Web.JoinPath;
        QrImage = await _qr.CreateAsync(JoinUrl);
        Tick();
    }

    private void Tick()
    {
        if (_room.OpenedAt is not { } openedAt) return;

        // Horas totales: no se reinicia a las 24 h
        var elapsed = _clock.Since(openedAt);
        Elapsed = string.Format(AppConfig.Settings.Ui.ElapsedFormat, (int)elapsed.TotalHours, elapsed.Minutes, elapsed.Seconds);
    }
}
