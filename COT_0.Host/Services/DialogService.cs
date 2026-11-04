using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace COT_0.Host.Services;

// Diálogos simples (manual, errores y selector de archivos)
public class DialogService(IConfiguration configuration)
{
    public void ShowManual() => MessageBox.Show(
        Application.Current.MainWindow,
        configuration["Texts:Manual:Text"],
        configuration["Texts:Manual:Title"],
        MessageBoxButton.OK,
        MessageBoxImage.Information);

    public void ShowError(string title, string message) => MessageBox.Show(
        Application.Current.MainWindow,
        message,
        $"{configuration["Texts:App:ErrorTitle"]} · {title}",
        MessageBoxButton.OK,
        MessageBoxImage.Error);

    // Selector de varios archivos filtrado por extensión; vacío si se cancela
    public IReadOnlyList<string> PickFiles(string filterName, IEnumerable<string> extensions)
    {
        var patterns = string.Join(';', extensions.Select(extension => "*" + extension));
        var dialog = new OpenFileDialog { Multiselect = true, Filter = $"{filterName} ({patterns})|{patterns}" };
        return dialog.ShowDialog() == true ? dialog.FileNames : [];
    }
}
