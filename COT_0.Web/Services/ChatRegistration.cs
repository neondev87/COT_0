using COT_0.Core.Services;
using COT_0.Web.Hubs;
using COT_0.Web.Options;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

// Registro de servicios y endpoints del chat
public static class ChatRegistration
{
    public static IServiceCollection AddChat(this IServiceCollection services, IConfiguration config, string serverUrl)
    {
        services.Configure<ChatOptions>(config.GetSection(ChatOptions.Section));
        services.PostConfigure<ChatOptions>(o => o.ServerUrl = serverUrl);
        services.Configure<VoiceOptions>(config.GetSection(VoiceOptions.Section));

        // Cuánto espera el servidor a un celular que se desconectó (pantalla bloqueada)
        services.AddOptions<CircuitOptions>().Configure<IOptions<ChatOptions>>((circuit, chat) =>
            circuit.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(chat.Value.CircuitRetentionMinutes));

        // Misma instancia para el chat, el feed de mensajes y editar perfiles
        services.AddSingleton<InMemoryChatService>();
        services.AddSingleton<IChatService>(sp => sp.GetRequiredService<InMemoryChatService>());
        services.AddSingleton<IUserEditor>(sp => sp.GetRequiredService<InMemoryChatService>());
        services.AddSingleton<IChatFeed>(sp => sp.GetRequiredService<InMemoryChatService>());
        services.AddSingleton<IPresenceTracker, PresenceTracker>();
        services.AddSingleton<IMessageFormatter, MessageFormatter>();
        services.AddSingleton<IVoiceStore, InMemoryVoiceStore>();
        services.AddSingleton<IAudioTranscoder, OpusWavTranscoder>();
        services.AddSingleton<IVoiceService, VoiceService>();
        services.AddSingleton<IRoomService, RoomService>();
        services.AddSingleton<IAvatarStore, InMemoryAvatarStore>();
        services.AddSingleton<IEmoteService, EmoteService>();
        services.AddSingleton<IFlagService, FlagService>();
        services.AddScoped<IGuestStore, GuestStore>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IChatSession, ChatSession>();
        services.AddScoped<IChatBrowser, ChatBrowser>();
        services.AddScoped<IVoiceRecorder, VoiceRecorder>();
        services.AddHostedService<ChatBroadcaster>();
        return services;
    }

    public static WebApplication MapChat(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<ChatOptions>>().Value;

        app.MapHub<ChatHub>(options.HubPath);
        app.MapGet($"{options.AvatarsPath.TrimEnd('/')}/{{id}}", (string id, IAvatarStore store) =>
            store.Get(id) is { } avatar ? Results.File(avatar.Bytes, avatar.ContentType) : Results.NotFound());

        // Notas de voz: con Range (206), que Safari/iPhone exige para reproducir
        var voice = app.Services.GetRequiredService<IOptions<VoiceOptions>>().Value;
        app.MapGet($"{voice.Path.TrimEnd('/')}/{{id}}", (string id, IVoiceStore store) =>
            store.Get(id) is { } clip ? Results.File(clip.Bytes, clip.ContentType, enableRangeProcessing: true) : Results.NotFound());
        return app;
    }
}
