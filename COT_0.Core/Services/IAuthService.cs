namespace COT_0.Core.Services;

// Login del host (credenciales en appsettings.json: Host:User y Host:Password)
public interface IAuthService
{
    bool IsAuthenticated { get; }

    Task<bool> LoginAsync(string user, string password);
    void Logout();
}
