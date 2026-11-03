using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace COT_0.Host.Services;

// Cambia entre la ventana de login y la principal; si el usuario cierra una, se cierra la app
public class WindowNavigator(IServiceProvider services)
{
    private Window? _current;

    public void ShowLogin() => Show<LoginWindow>();

    public void ShowMain() => Show<MainWindow>();

    private void Show<T>() where T : Window
    {
        var next = services.GetRequiredService<T>();
        var previous = _current;
        _current = next;

        Application.Current.MainWindow = next;
        next.Closed += OnWindowClosed;
        next.Show();

        if (previous is null) return;
        previous.Closed -= OnWindowClosed;
        previous.Close();
    }

    private void OnWindowClosed(object? sender, EventArgs e) => Application.Current.Shutdown();
}
