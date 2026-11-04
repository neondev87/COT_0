using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services.Emotes;

// Registro de los emotes de 7TV, aparte del resto del chat
public static class EmoteRegistration
{
    public static IServiceCollection AddRoomEmotes(this IServiceCollection services)
    {
        // El cliente vive dentro de un Singleton: conexiones que se renuevan solas en vez de rotar el handler
        services.AddHttpClient<ISevenTvClient, SevenTvClient>((sp, http) =>
            {
                var o = sp.GetRequiredService<IOptions<ChatOptions>>().Value;
                http.BaseAddress = new Uri(o.SevenTvApiUrl);
                http.Timeout = TimeSpan.FromSeconds(o.EmoteRequestTimeoutSeconds);
                http.DefaultRequestHeaders.UserAgent.ParseAdd(o.SevenTvUserAgent);
            })
            .ConfigurePrimaryHttpMessageHandler(sp => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(sp.GetRequiredService<IOptions<ChatOptions>>().Value.EmoteCacheMinutes),
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

        services.AddSingleton<IRoomEmotes, RoomEmotes>();
        services.AddScoped<IEmoteActions, EmoteActions>();
        return services;
    }
}
