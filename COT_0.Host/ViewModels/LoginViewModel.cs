using COT_0.Core.Services;
using COT_0.Host.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.ViewModels;

// Login del host
public partial class LoginViewModel(
    IAuthService auth,
    WindowNavigator windows,
    SignatureViewModel signature,
    IConfiguration configuration) : ObservableObject
{
    [ObservableProperty]
    private string _user = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _error = string.Empty;

    public SignatureViewModel Signature => signature;

    [RelayCommand]
    private async Task LoginAsync()
    {
        Error = string.Empty;
        if (await auth.LoginAsync(User, Password))
        {
            StartupTrace.Mark("Login aceptado");
            StartupTrace.Measure("Ventana principal: crear y mostrar", windows.ShowMain);
            return;
        }

        Error = configuration.GetText("Login:Error");
        Password = string.Empty;
    }
}
