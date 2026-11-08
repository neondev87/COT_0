// Sala: al entrar pide abrirla (servidor + túnel + QR); la lógica vive en SalaViewModel
using COT_0.HostWinUI.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Pantallas;

public sealed partial class SalaPage : Page
{
    public SalaViewModel ViewModel { get; } = App.Services.GetRequiredService<SalaViewModel>();

    public SalaPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e) => await ViewModel.EnsureOpenAsync();
}
