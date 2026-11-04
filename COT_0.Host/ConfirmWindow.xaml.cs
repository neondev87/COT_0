using COT_0.Host.Controls;

namespace COT_0.Host;

// Textos del diálogo de confirmación
public record ConfirmTexts(string Message, string Confirm, string Cancel);

// Diálogo Sí/No con el estilo de COT_0; CANCELAR, Esc y ✕ devuelven false
public partial class ConfirmWindow : ChromeWindow
{
    public ConfirmWindow(ConfirmTexts texts)
    {
        InitializeComponent();
        DataContext = texts;
    }

    private void OnConfirm(object sender, System.Windows.RoutedEventArgs e) => DialogResult = true;
}
