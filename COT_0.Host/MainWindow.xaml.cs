using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using COT_0.Core.Services;
using COT_0.Host.Services;

namespace COT_0.Host;

// Ventana principal: botón "Generar QR", túnel y QR
public partial class MainWindow : Window
{
    // Ruta de la página de ingreso (ver CONTRATO.md)
    private const string JoinPath = "/join";

    private readonly WebServerHost _webServer;
    private readonly ITunnelService _tunnel;
    private readonly QrService _qr;

    public MainWindow(WebServerHost webServer, ITunnelService tunnel, QrService qr)
    {
        _webServer = webServer;
        _tunnel = tunnel;
        _qr = qr;
        InitializeComponent();
    }

    private async void OnGenerateClick(object sender, RoutedEventArgs e)
    {
        GenerateButton.IsEnabled = false;
        StatusText.Text = "Levantando servidor y túnel...";

        try
        {
            await _webServer.StartAsync();
            var joinUrl = await _tunnel.StartAsync(WebServerHost.Url) + JoinPath;

            QrImage.Source = _qr.Generate(joinUrl);
            UrlBox.Text = joinUrl;
            ResultPanel.Visibility = Visibility.Visible;
            StatusText.Text = "Túnel activo. Puede tardar unos segundos en responder la primera vez.";
        }
        catch (Exception ex)
        {
            StatusText.Text = "No se pudo generar el QR.";
            MessageBox.Show(this, ex.Message, "COT_0", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            GenerateButton.IsEnabled = true;
        }
    }

    private void OnCopyClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Clipboard.SetText(UrlBox.Text);
            StatusText.Text = "URL copiada al portapapeles.";
        }
        catch (COMException)
        {
            StatusText.Text = "El portapapeles está ocupado, probá de nuevo.";
        }
    }

    // Al cerrar se mata el proceso de cloudflared
    private async void OnClosing(object? sender, CancelEventArgs e) => await _tunnel.StopAsync();
}
