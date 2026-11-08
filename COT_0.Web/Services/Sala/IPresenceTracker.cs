namespace COT_0.Web.Services.Sala;

/// <summary>Saca a un invitado que perdió la conexión solo si no vuelve dentro de Chat:LeaveGraceSeconds</summary>
public interface IPresenceTracker
{
    void ScheduleLeave(string userId, string connectionId);
}
