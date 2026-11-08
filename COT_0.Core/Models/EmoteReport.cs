namespace COT_0.Core.Models;

/// <summary>Reporte de un invitado sobre un emote; lo revisa el host</summary>
public sealed record EmoteReport(RoomEmote Emote, string UserId, string UserName, DateTime ReportedAt);
