namespace COT_0.Web.Services.Emotes;

// Reporte de un invitado sobre un emote; lo revisa el host
public sealed record EmoteReport(RoomEmote Emote, string UserId, string UserName, DateTime ReportedAt);
