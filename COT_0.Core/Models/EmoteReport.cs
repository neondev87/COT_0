namespace COT_0.Core.Models;

// Reporte de un invitado sobre un emote; lo revisa el host
public sealed record EmoteReport(RoomEmote Emote, string UserId, string UserName, DateTime ReportedAt);
