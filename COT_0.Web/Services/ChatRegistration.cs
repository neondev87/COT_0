using COT_0.Core.Services;
using COT_0.Web.Hubs;
using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

// Registro de servicios y endpoints del chat
public static class ChatRegistration
{
    public static IServiceCollection AddChat(this IServiceCollection services, IConfiguration config, string serverUrl)
    {
        services.Configure<ChatOptions>(config.GetSection(ChatOptions.Section));
        services.PostConfigure<ChatOptions>(o => o.ServerUrl = serverUrl);

        services.AddSingleton<IChatService, InMemoryChatService>();
        services.AddSingleton<IRoomService, RoomService>();
        services.AddSingleton<IAvatarStore, InMemoryAvatarStore>();
        services.AddSingleton<IEmoteService, EmoteService>();
        services.AddSingleton<IFlagService, FlagService>();
        services.AddScoped<IGuestStore, GuestStore>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IChatSession, ChatSession>();
        services.AddHostedService<ChatBroadcaster>();
        return services;
    }

    public static WebApplication MapChat(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<ChatOptions>>().Value;

        app.MapHub<ChatHub>(options.HubPath);
        app.MapGet($"{options.AvatarsPath.TrimEnd('/')}/{{id}}", (string id, IAvatarStore store) =>
            store.Get(id) is { } avatar ? Results.File(avatar.Bytes, avatar.ContentType) : Results.NotFound());
        return app;
    }
}
