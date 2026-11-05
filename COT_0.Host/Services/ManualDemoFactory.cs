using COT_0.Core.Models;
using COT_0.Host.ViewModels;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Arma los datos de muestra del manual (Manual:Demo) con los mismos formatos y textos de la app real
public class ManualDemoFactory(IConfiguration configuration, QrService qr, ClockService clock, CoverLibrary covers, CoverDecoder decoder)
{
    public ManualDemo Create()
    {
        var signature = new DemoSignature(true, false, Demo.GetValue<double>("Volume"), false, CreateSongs());
        var room = CreateRoom();
        var connected = string.Format(configuration.GetText("Dashboard:Connected"), room.Total);
        var confirm = new ConfirmTexts(configuration.GetText("ExitDialog:Message"), configuration.GetText("ExitDialog:Confirm"), configuration.GetText("ExitDialog:Cancel"));
        return new ManualDemo(CreateDashboard(false, connected, signature), CreateDashboard(true, connected, signature), room, confirm);
    }

    private IConfigurationSection Demo => configuration.GetSection("Manual:Demo");

    private DemoDashboard CreateDashboard(bool active, string connected, DemoSignature signature)
    {
        var culture = configuration.GetUiCulture();
        var now = clock.Now;
        return new DemoDashboard(
            now.ToString(configuration["Ui:ClockFormat"], culture),
            now.ToString(configuration["Ui:SecondsFormat"], culture),
            now.ToString(configuration["Ui:DateFormat"], culture).ToUpper(culture),
            active, connected,
            configuration.GetText(active ? "Dashboard:RoomTitle" : "Dashboard:QrTitle"),
            configuration.GetText(active ? "Dashboard:RoomSubtitle" : "Dashboard:QrSubtitle"),
            signature);
    }

    private DemoRoom CreateRoom()
    {
        var joinUrl = Demo["JoinUrl"] ?? string.Empty;
        var sinceFormat = configuration.GetText("Room:Since");
        var users = Demo.GetSection("Guests").GetChildren()
            .Select(guest => new User { Name = guest["Name"] ?? string.Empty, JoinedAt = DateTime.UtcNow.AddMinutes(-guest.GetValue<int>("Minutes")) })
            .Select(user => new UserRowViewModel(user, sinceFormat, clock))
            .ToList();
        return new DemoRoom(Demo["Code"] ?? string.Empty, Demo["Elapsed"] ?? string.Empty, joinUrl, qr.Generate(joinUrl), false,
            users.Count, users, CreateEmotes());
    }

    private DemoEmotes CreateEmotes()
    {
        var reportFormat = configuration.GetText("Emotes:ReportCount");
        var reports = Demo.GetSection("Reports").GetChildren()
            .Select(report => new EmoteRowViewModel(Emote(report["Name"]), string.Format(reportFormat, report.GetValue<int>("Count"))))
            .ToList();
        var removed = (Demo.GetSection("Removed").Get<string[]>() ?? [])
            .Select(name => new EmoteRowViewModel(Emote(name), configuration.GetText("Emotes:RemovedDetail")))
            .ToList();
        return new DemoEmotes(Demo["EmoteSet"] ?? string.Empty, EmoteSetStatus.Ready, configuration.GetText("Emotes:Status:Ready"),
            string.Format(configuration.GetText("Emotes:Count"), Demo.GetValue<int>("EmoteCount")), reports, removed);
    }

    // Sin imagen: la muestra no descarga nada de 7TV
    private static RoomEmote Emote(string? name)
    {
        var none = new EmoteSources(string.Empty, null, null, string.Empty);
        return new RoomEmote(name ?? string.Empty, name ?? string.Empty, 0, 0, false, none, none);
    }

    // Títulos de muestra con las portadas reales (misma rotación que el selector)
    private DemoSongs CreateSongs()
    {
        var available = covers.GetCovers();
        var songs = Demo.GetSection("Songs").GetChildren()
            .Select((song, position) =>
            {
                var title = song["Title"] ?? string.Empty;
                var cover = CoverLibrary.Resolve(available, title, position);
                return new SongItem(string.Empty, title, song["Duration"] ?? string.Empty, cover is null ? null : decoder.Decode(cover));
            })
            .ToList();

        var playing = songs.ElementAtOrDefault(Demo.GetValue<int>("PlayingSong"));
        if (playing is not null) playing.IsPlaying = true;
        return new DemoSongs(true, string.Empty, songs, playing);
    }
}
