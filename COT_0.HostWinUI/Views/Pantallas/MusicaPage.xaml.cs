// Pantalla de música: carga la biblioteca al abrirse y pasa el clic de una fila al ViewModel
using COT_0.HostWinUI.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Pantallas;

public sealed partial class MusicaPage : Page
{
    public MusicaViewModel ViewModel { get; } = App.Services.GetRequiredService<MusicaViewModel>();

    public MusicaPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e) => await ViewModel.LoadAsync();

    private void OnSongClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is SongRowViewModel song) ViewModel.PlayCommand.Execute(song);
    }
}
