// Capa del fondo detrás del Frame: imagen, cuadro fijo o video según el fondo elegido y la página visible
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Fondos;
using COT_0.Host.Services.Ventana;
using COT_0.Host.Views.Pantallas;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using Windows.Media.Playback;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Solo se ve en Dashboard, Sala y Fondos; en las demás páginas el
/// video se pausa. Sin desenfoque: con el selector abierto el velo oscuro sube (PickerVeilOpacity).
/// Si el video falla, cae a su cuadro fijo (el JPEG de la caché) y FondoService guarda la causa para FondosPage.
/// </summary>
public partial class CapaFondoViewModel : ObservableObject
{
    private static readonly HashSet<Type> PagesWithBackground = [typeof(DashboardPage), typeof(SalaPage), typeof(FondosPage)];

    private readonly FondoService _fondos;
    private readonly FondoPlayer _player;
    private readonly FondoImages _images;
    private readonly HashSet<string> _failedVideos = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _failedStills = new(StringComparer.OrdinalIgnoreCase);
    private string? _stillPath;
    private bool _isPicker;
    private int _version;

    [ObservableProperty]
    private bool _isActive;

    [ObservableProperty]
    private bool _isVideoVisible;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStillVisible))]
    private ImageSource? _still;

    [ObservableProperty]
    private double _veilOpacity;

    public CapaFondoViewModel(FondoService fondos, FondoPlayer player, FondoImages images)
    {
        _fondos = fondos;
        _player = player;
        _images = images;
        Navigator.PageChanged += OnPageChanged;
        fondos.Changed += () => _ = ApplyAsync();
        player.Failed += OnVideoFailed;
    }

    public MediaPlayer Player => _player.Player;

    public bool IsStillVisible => Still is not null && !IsVideoVisible;

    private static BackgroundSettings Settings => AppConfig.Settings.Background;

    // La carpeta se lee la primera vez que se necesita: el login arranca sin tocar los fondos
    private async void OnPageChanged(Type page)
    {
        IsActive = PagesWithBackground.Contains(page);
        _isPicker = page == typeof(FondosPage);
        // Abrir el selector vuelve a intentar los archivos que fallaron (pudiste reemplazarlos)
        if (_isPicker) ForgetFailures();
        if (IsActive && !_fondos.IsLoaded) await _fondos.LoadAsync();
        else await ApplyAsync();
    }

    private async Task ApplyAsync()
    {
        var version = ++_version;
        var fondo = _fondos.Selected;
        VeilOpacity = _isPicker ? Settings.PickerVeilOpacity : fondo is null ? 0 : Settings.VeilOpacity;

        if (!IsActive)
        {
            _player.Pause();
            return;
        }
        if (fondo is { Kind: FondoKind.Video, UseStill: false } && !_failedVideos.Contains(_fondos.PathOf(fondo)))
        {
            await ShowVideoAsync(_fondos.PathOf(fondo));
            return;
        }

        _player.Stop();
        IsVideoVisible = false;
        await ShowStillAsync(fondo, version);
    }

    private async Task ShowVideoAsync(string path)
    {
        IsVideoVisible = true;
        await _player.PlayAsync(path);
        OnPropertyChanged(nameof(IsStillVisible));
    }

    // Imagen o cuadro fijo; no se vuelve a decodificar si es el mismo archivo
    private async Task ShowStillAsync(Fondo? fondo, int version)
    {
        var path = fondo is null ? null : _fondos.PathOf(fondo);
        if (path == _stillPath) return;

        var image = fondo is null || path is null || _failedStills.Contains(path)
            ? null
            : await _images.LoadAsync(path, fondo.Kind, Settings.StillSize);
        if (version != _version) return;

        _stillPath = path;
        Still = image;
        if (fondo is null || path is null || image is not null || !_failedStills.Add(path)) return;
        _fondos.ReportFailure(fondo, fondo.Kind == FondoKind.Video ? FondoError.NoFrame : FondoError.Unreadable, null);
    }

    // Video que no se pudo abrir: el aviso dispara ApplyAsync, que ya lo trata como cuadro fijo
    private void OnVideoFailed(FondoError error, string code)
    {
        if (_fondos.Selected is not { } fondo || !_failedVideos.Add(_fondos.PathOf(fondo))) return;
        _player.Stop();
        _fondos.ReportFailure(fondo, error, code);
    }

    private void ForgetFailures()
    {
        if (_failedVideos.Count == 0 && _failedStills.Count == 0) return;
        _failedVideos.Clear();
        _failedStills.Clear();
        _stillPath = null;
        Still = null;
    }
}
