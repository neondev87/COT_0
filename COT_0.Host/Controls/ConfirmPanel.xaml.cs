using System.Windows;
using System.Windows.Controls;

namespace COT_0.Host.Controls;

// Mensaje + CANCELAR / confirmar; avisa con Confirmed
public partial class ConfirmPanel : UserControl
{
    public ConfirmPanel() => InitializeComponent();

    public event RoutedEventHandler? Confirmed;

    private void OnConfirm(object sender, RoutedEventArgs e) => Confirmed?.Invoke(this, e);
}
