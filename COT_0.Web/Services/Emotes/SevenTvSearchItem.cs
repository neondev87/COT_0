namespace COT_0.Web.Services.Emotes;

/// <summary>Emote de la búsqueda: mismo host/archivos que el de un set, así se traduce con SevenTvMapper.</summary>
internal sealed record SevenTvSearchItem(string Id, string Name, bool Animated, SevenTvHost? Host)
{
    public SevenTvActiveEmote ToActive() => new(Id, Name, new SevenTvEmoteData(Animated, Host));
}
