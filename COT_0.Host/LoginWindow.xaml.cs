using COT_0.Host.Controls;
using COT_0.Host.ViewModels;

namespace COT_0.Host;

// Login del host (lógica en LoginViewModel)
public partial class LoginWindow : ChromeWindow
{
    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
