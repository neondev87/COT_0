using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace COT_0.Host.Services;

// Cambia entre la ventana de login y la principal; si el usuario cierra una, se cierra la app.
// Todo cierre del usuario (✕, Alt+F4, barra de tareas) pasa por el coordinador de cierre
public class WindowNavigator(IServiceProvider services, IShutdownCoordinator shutdown)
{
    private Window? _current;

    public void ShowLogin() => Show<LoginWindow>();

    public void ShowMain() => Show<MainWindow>();

    // Manual de Lain sobre la ventana actual, del mismo tamaño
    public void ShowManual()
    {
        var manual = services.GetRequiredService<ManualWindow>();
        if (_current is { } owner)
        {
            manual.Owner = owner;
            manual.Width = owner.ActualWidth;
            manual.Height = owner.ActualHeight;
            manual.WindowState = owner.WindowState;
        }
        manual.ShowDialog();
    }

    private void Show<T>() where T : Window
    {
        var next = services.GetRequiredService<T>();
        var previous = _current;
        _current = next;

        Application.Current.MainWindow = next;
        next.Closing += OnWindowClosing;
        next.Closed += OnWindowClosed;
        next.Show();

        if (previous is null) return;
        previous.Closing -= OnWindowClosing;
        previous.Closed -= OnWindowClosed;
        previous.Close();
    }

    private void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (sender is Window window) shutdown.OnWindowClosing(window, e);
    }

    private void OnWindowClosed(object? sender, EventArgs e) => Application.Current.Shutdown();
}
