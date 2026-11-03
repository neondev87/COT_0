using System.Windows;
using System.Windows.Input;

namespace COT_0.Host.Controls;

// Ventana sin borde (WindowChrome en Themes/Window.xaml) con comandos de los botones propios
public class ChromeWindow : Window
{
    private const string StyleKey = "ChromeWindowStyle";

    public ChromeWindow()
    {
        SetResourceReference(StyleProperty, StyleKey);
        CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, (_, _) => SystemCommands.MinimizeWindow(this)));
        CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, (_, _) => ToggleMaximize(), CanResize));
        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (_, _) => SystemCommands.CloseWindow(this)));
    }

    private void CanResize(object sender, CanExecuteRoutedEventArgs e) =>
        e.CanExecute = ResizeMode is ResizeMode.CanResize or ResizeMode.CanResizeWithGrip;

    private void ToggleMaximize()
    {
        if (WindowState == WindowState.Maximized) SystemCommands.RestoreWindow(this);
        else SystemCommands.MaximizeWindow(this);
    }
}
