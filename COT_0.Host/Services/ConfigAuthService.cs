using COT_0.Core.Services;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Login del host contra Host:User / Host:Password de appsettings.json
public class ConfigAuthService(IConfiguration configuration) : IAuthService
{
    public bool IsAuthenticated { get; private set; }

    public Task<bool> LoginAsync(string user, string password)
    {
        var expectedUser = configuration["Host:User"];
        var expectedPassword = configuration["Host:Password"];

        IsAuthenticated = !string.IsNullOrEmpty(expectedUser)
            && user == expectedUser
            && password == expectedPassword;

        return Task.FromResult(IsAuthenticated);
    }

    public void Logout() => IsAuthenticated = false;
}
