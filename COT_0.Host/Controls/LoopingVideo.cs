using System.Windows;
using System.Windows.Controls;
using COT_0.Host.Services;

namespace COT_0.Host.Controls;

// Video de fondo en loop y sin sonido; con la ventana minimizada se pausa para no decodificar de más
public class LoopingVideo : MediaElement
{
    private Window? _window;

    public LoopingVideo()
    {
        LoadedBehavior = MediaState.Manual;
        UnloadedBehavior = MediaState.Close;
        IsMuted = true;
        Loaded += (_, _) => Attach();
        Unloaded += (_, _) => Detach();
        MediaOpened += (_, _) => StartupTrace.Mark("  Video de fondo: abierto");
        MediaEnded += (_, _) =>
        {
            Position = TimeSpan.Zero;
            Play();
        };
    }

    private void Attach()
    {
        StartupTrace.Mark("  Video de fondo: Play");
        _window = Window.GetWindow(this);
        if (_window is not null) _window.StateChanged += OnWindowStateChanged;
        Play();
    }

    private void Detach()
    {
        if (_window is not null) _window.StateChanged -= OnWindowStateChanged;
        _window = null;
    }

    private void OnWindowStateChanged(object? sender, EventArgs e)
    {
        if (_window?.WindowState == WindowState.Minimized) Pause();
        else Play();
    }
}
