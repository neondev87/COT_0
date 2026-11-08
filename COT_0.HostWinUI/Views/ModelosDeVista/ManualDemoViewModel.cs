// Datos de muestra de las vistas de ejemplo del manual (Manual:Demo), con los mismos formatos que la app real
using System.Globalization;
using COT_0.Core.Models;
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Configuracion;
using COT_0.HostWinUI.Services.Textos;
using COT_0.HostWinUI.Services.Tiempo;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>
/// Reemplaza al ManualDemoFactory del WPF. Todo es ficticio: no toca la sala, el túnel, 7TV ni la música.
/// La hora y la fecha salen del reloj al abrir el manual; lo demás, de appsettings.json.
/// </summary>
public sealed class ManualDemoViewModel
{
    private const string TotalFormat = "00";

    public ManualDemoViewModel(ClockService clock)
    {
        var demo = AppConfig.Settings.Manual.Demo;
        var ui = AppConfig.Settings.Ui;
        var culture = CultureInfo.GetCultureInfo(ui.Culture);
        var now = clock.Now;

        Time = now.ToString(ui.ClockFormat, culture);
        Seconds = now.ToString(ui.SecondsFormat, culture);
        Date = now.ToString(ui.DateFormat, culture).ToUpper(culture);
        User = demo.User;
        Code = demo.Code;
        Elapsed = demo.Elapsed;
        JoinUrl = demo.JoinUrl;

        var sinceFormat = UiText.Get("Text.Room.Since");
        Guests = demo.Guests
            .Select((guest, i) => new UserRowViewModel(
                new User { Name = guest.Name, JoinedAt = DateTime.UtcNow.AddMinutes(-guest.Minutes) }, i, sinceFormat, clock))
            .ToList();
        TotalText = Guests.Count.ToString(TotalFormat, CultureInfo.InvariantCulture);

        EmoteSet = demo.EmoteSet;
        EmoteCountText = string.Format(culture, UiText.Get("Text.Emotes.Count"), demo.EmoteCount);
        EmoteStatusText = UiText.Get("Text.Emotes.Status.Ready");
        Reports = demo.Reports
            .Select(r => new ManualDemoRow(r.Name, string.Format(culture, UiText.Get("Text.Emotes.ReportCount"), r.Count), false))
            .ToList();
        Removed = demo.Removed.Select(name => new ManualDemoRow(name, UiText.Get("Text.Emotes.RemovedDetail"), false)).ToList();

        Songs = demo.Songs.Select((song, i) => new ManualDemoRow(song.Title, song.Duration, i == demo.PlayingSong)).ToList();
        PlayingTitle = Songs.FirstOrDefault(s => s.IsMarked)?.Title ?? string.Empty;
        Volume = demo.Volume;
        VolumeText = Math.Round(demo.Volume).ToString(CultureInfo.InvariantCulture);
    }

    public string Time { get; }
    public string Seconds { get; }
    public string Date { get; }
    public string User { get; }
    public string Code { get; }
    public string Elapsed { get; }
    public string JoinUrl { get; }
    public IReadOnlyList<UserRowViewModel> Guests { get; }
    public string TotalText { get; }
    public string EmoteSet { get; }
    public string EmoteCountText { get; }
    public EmoteSetStatus EmoteStatus => EmoteSetStatus.Ready;
    public string EmoteStatusText { get; }
    public IReadOnlyList<ManualDemoRow> Reports { get; }
    public IReadOnlyList<ManualDemoRow> Removed { get; }
    public IReadOnlyList<ManualDemoRow> Songs { get; }
    public string PlayingTitle { get; }
    public double Volume { get; }
    public string VolumeText { get; }
}
