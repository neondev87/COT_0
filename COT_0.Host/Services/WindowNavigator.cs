using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace COT_0.Host.Services;

// Cambia entre la ventana de login y la principal; si el usuario cierra una, se cierra la app
public class WindowNavigator(IServiceProvider services, RoomSession session)
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
        next.Closing += OnWindowClosing;
        next.Closed += OnWindowClosed;
        next.Show();

        if (previous is null) return;
        previous.Closing -= OnWindowClosing;
        previous.Closed -= OnWindowClosed;
        previous.Close();
    }

    // ✕ o Alt+F4 con sala activa: se cancela, se confirma y se cierra la sala antes de salir
    private async void OnWindowClosing(object? sender, CancelEventArgs e)
    {
        if (!session.IsActive || sender is not Window window) return;

        e.Cancel = true;
        if (await session.ConfirmCloseAsync()) window.Close();
    }

    private void OnWindowClosed(object? sender, EventArgs e) => Application.Current.Shutdown();
}
