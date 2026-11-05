using COT_0.Host.Controls;
using COT_0.Host.ViewModels;

namespace COT_0.Host;

// Manual de Lain (lógica en ManualViewModel)
public partial class ManualWindow : ChromeWindow
{
    public ManualWindow(ManualViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseRequested += Close;
        Closed += (_, _) => viewModel.Dispose();
    }
}
