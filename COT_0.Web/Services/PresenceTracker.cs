using COT_0.Core.Services;
using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

public sealed class PresenceTracker(IChatService chat, IOptions<ChatOptions> options, ILogger<PresenceTracker> logger)
    : IPresenceTracker
{
    private readonly TimeSpan _grace = TimeSpan.FromSeconds(options.Value.LeaveGraceSeconds);

    public void ScheduleLeave(string userId, string connectionId) => _ = LeaveLaterAsync(userId, connectionId);

    private async Task LeaveLaterAsync(string userId, string connectionId)
    {
        try
        {
            await Task.Delay(_grace);

            // Si se reconectó (o recargó la página) ya tiene otra conexión y se queda
            var user = await chat.GetUserAsync(userId);
            if (user?.ConnectionId == connectionId) await chat.RemoveUserAsync(userId);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "No se pudo sacar al usuario {UserId}", userId);
        }
    }
}
