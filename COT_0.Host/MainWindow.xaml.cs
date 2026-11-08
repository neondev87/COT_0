// Ventana principal: la prepara, le da su Frame al Navigator y abre el login
using COT_0.Host.Services.Ventana;
using Microsoft.UI.Xaml;

namespace COT_0.Host;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowSetup.Apply(this);
        Navigator.Attach(RootFrame);
        Navigator.ShowLogin();
    }
}
