using COT_0.Host.Services.Ventana;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace COT_0.Host;

/// <summary>Ventana principal: la prepara, le da su Frame al Navigator y abre el login</summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        WindowSetup.Apply(this);
        Navigator.Attach(RootFrame);
        Navigator.ShowLogin();
    }

    private void OnCloseClick(object sender, RoutedEventArgs e) => _ = App.Services.GetRequiredService<CloseGuard>().RequestCloseAsync();
}
