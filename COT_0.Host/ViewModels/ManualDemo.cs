using COT_0.Core.Models;
using System.Windows.Media;

namespace COT_0.Host.ViewModels;

// Datos de muestra para las vistas reales dentro del manual: mismas propiedades que sus ViewModels,
// sin sala, túnel ni comandos (el escenario del manual no se puede tocar)
public sealed record ManualDemo(DemoDashboard Idle, DemoDashboard Active, DemoRoom Room, ConfirmTexts Confirm);

public sealed record DemoDashboard(string Time, string Seconds, string Date, bool IsRoomActive, string ConnectedText,
    string QrTitle, string QrSubtitle, DemoSignature Signature);

public sealed record DemoSignature(bool IsMusicAvailable, bool IsMuted, double Volume, bool IsUploadOpen, DemoSongs Songs);

public sealed record DemoSongs(bool IsOpen, string Query, IReadOnlyList<SongItem> Songs, SongItem? Selected);

public sealed record DemoRoom(string Code, string Elapsed, string JoinUrl, ImageSource QrImage, bool IsCopied, int Total,
    IReadOnlyList<UserRowViewModel> Users, DemoEmotes Emotes);

public sealed record DemoEmotes(string SetInput, EmoteSetStatus Status, string StatusText, string CountText,
    IReadOnlyList<EmoteRowViewModel> Reports, IReadOnlyList<EmoteRowViewModel> Removed)
{
    public bool HasReports => Reports.Count > 0;
    public bool HasRemoved => Removed.Count > 0;
}
