using COT_0.Host.Controls;
using COT_0.Host.ViewModels;

namespace COT_0.Host;

// Ventana principal: dashboard y sala (lógica en los ViewModels)
public partial class MainWindow : ChromeWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
