namespace COT_0.Web.Services.Emotes;

// Cliente de la API pública v3 de 7TV (GET /emote-sets/{id})
public interface ISevenTvClient
{
    Task<EmoteSetResult> GetEmoteSetAsync(string setId, CancellationToken ct = default);
}
