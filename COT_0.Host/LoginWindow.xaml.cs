using System.Windows;
using COT_0.Core.Services;

namespace COT_0.Host;

// Login del host (usa IAuthService)
public partial class LoginWindow : Window
{
    private readonly IAuthService _auth;

    public LoginWindow(IAuthService auth)
    {
        _auth = auth;
        InitializeComponent();
    }

    private async void OnLoginClick(object sender, RoutedEventArgs e)
    {
        LoginButton.IsEnabled = false;
        ErrorText.Text = string.Empty;

        if (await _auth.LoginAsync(UserBox.Text, PasswordBox.Password))
        {
            DialogResult = true;
            return;
        }

        ErrorText.Text = "Usuario o contraseña incorrectos.";
        PasswordBox.Clear();
        PasswordBox.Focus();
        LoginButton.IsEnabled = true;
    }
}
