// Pantalla de fondos: carga el catálogo al abrirse y pasa el clic de una miniatura al ViewModel
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Pantallas;

public sealed partial class FondosPage : Page
{
    public FondosViewModel ViewModel { get; } = App.Services.GetRequiredService<FondosViewModel>();

    public FondosPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e) => await ViewModel.LoadAsync();

    private void OnItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is FondoRowViewModel row) ViewModel.PickCommand.Execute(row);
    }
}
