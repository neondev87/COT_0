// Panel de emotes: toma su ViewModel del contenedor de servicios; los botones de cada fila solo le pasan su emote
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

public sealed partial class PanelEmotes : UserControl
{
    public EmotesViewModel ViewModel { get; } = App.Services.GetRequiredService<EmotesViewModel>();

    public PanelEmotes() => InitializeComponent();

    private void OnRemoveClick(object sender, RoutedEventArgs e) => Run(sender, ViewModel.RemoveCommand);

    private void OnIgnoreClick(object sender, RoutedEventArgs e) => Run(sender, ViewModel.IgnoreCommand);

    private void OnRestoreClick(object sender, RoutedEventArgs e) => Run(sender, ViewModel.RestoreCommand);

    // Cada botón trae su fila en el DataContext (en WinUI no hay RelativeSource AncestorType)
    private static void Run(object sender, System.Windows.Input.ICommand command)
    {
        if (sender is FrameworkElement { DataContext: EmoteRowViewModel row }) command.Execute(row);
    }
}
