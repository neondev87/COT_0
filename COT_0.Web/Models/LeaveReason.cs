namespace COT_0.Web.Models;

/// <summary>Por qué un invitado vuelve a /join desde el chat (?reason=...)</summary>
public static class LeaveReason
{
    public const string Kicked = "kicked";
    public const string RoomClosed = "closed";

    public static string? ToNotice(string? reason) => reason switch
    {
        Kicked => "El anfitrión te expulsó de la sala.",
        RoomClosed => "La sala se cerró.",
        _ => null,
    };
}
