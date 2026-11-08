namespace COT_0.Web.Services.Emotes;

/// <summary>Host del CDN de 7TV y archivos disponibles del emote.</summary>
internal sealed record SevenTvHost(string Url, List<SevenTvFile>? Files);
