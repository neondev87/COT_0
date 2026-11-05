using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace COT_0.Host.Services;

// Diálogos simples (errores, confirmación y selector de archivos)
public class DialogService(IConfiguration configuration)
{
    public void ShowError(string title, string message) => MessageBox.Show(
        Application.Current.MainWindow,
        message,
        $"{configuration.GetText("App:ErrorTitle")} · {title}",
        MessageBoxButton.OK,
        MessageBoxImage.Error);

    // Confirmación con estilo propio; true si se eligió confirmar
    public bool Confirm(string message, string confirm, string cancel) =>
        new ConfirmWindow(new ConfirmTexts(message, confirm, cancel)) { Owner = Application.Current.MainWindow }.ShowDialog() == true;

    // Selector de varios archivos filtrado por extensión; vacío si se cancela
    public IReadOnlyList<string> PickFiles(string filterName, IEnumerable<string> extensions)
    {
        var patterns = string.Join(';', extensions.Select(extension => "*" + extension));
        var dialog = new OpenFileDialog { Multiselect = true, Filter = $"{filterName} ({patterns})|{patterns}" };
        return dialog.ShowDialog() == true ? dialog.FileNames : [];
    }
}
