namespace COT_0.Host.Services;

public enum Screen
{
    Dashboard,
    Room,
}

// Pantalla visible dentro de la ventana principal (dashboard o sala)
public class ScreenNavigator
{
    public event Action? Changed;

    public Screen Current { get; private set; } = Screen.Dashboard;

    public void ShowDashboard() => Show(Screen.Dashboard);

    public void ShowRoom() => Show(Screen.Room);

    private void Show(Screen screen)
    {
        if (Current == screen) return;
        Current = screen;
        Changed?.Invoke();
    }
}
