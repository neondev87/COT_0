// Lista de conectados: toma su ViewModel del contenedor de servicios; la lógica vive ahí
using COT_0.HostWinUI.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Componentes;

/// <summary>
/// Muestra los invitados de la sala. Con SampleUsers (vista de ejemplo del manual) muestra esos en su lugar
/// y la ✕ no hace nada.
/// </summary>
public sealed partial class ListaConectados : UserControl
{
    public static readonly DependencyProperty SampleUsersProperty = DependencyProperty.Register(
        nameof(SampleUsers), typeof(IReadOnlyList<UserRowViewModel>), typeof(ListaConectados),
        new PropertyMetadata(null, (d, e) => ((ListaConectados)d).ShowUsers()));

    public ListaConectados()
    {
        InitializeComponent();
        ShowUsers();
    }

    public ConnectedUsersViewModel ViewModel { get; } = App.Services.GetRequiredService<ConnectedUsersViewModel>();

    public IReadOnlyList<UserRowViewModel>? SampleUsers
    {
        get => (IReadOnlyList<UserRowViewModel>?)GetValue(SampleUsersProperty);
        set => SetValue(SampleUsersProperty, value);
    }

    private void ShowUsers() => Guests.ItemsSource = SampleUsers ?? (object)ViewModel.Users;

    // El botón ✕ de cada fila trae su invitado en el DataContext
    private void OnKickClick(object sender, RoutedEventArgs e)
    {
        if (SampleUsers is null && sender is FrameworkElement { DataContext: UserRowViewModel user })
            ViewModel.KickCommand.Execute(user);
    }
}
