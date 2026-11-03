using System.Windows;
using Microsoft.Extensions.Configuration;

namespace COT_0.Host.Services;

// Diálogos simples (manual y errores)
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
}
