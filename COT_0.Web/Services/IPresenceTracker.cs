namespace COT_0.Web.Services;

// Saca a un invitado que perdió la conexión solo si no vuelve dentro de Chat:LeaveGraceSeconds
public interface IPresenceTracker
{
    void ScheduleLeave(string userId, string connectionId);
}
