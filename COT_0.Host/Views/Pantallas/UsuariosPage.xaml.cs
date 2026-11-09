using COT_0.Host.Views.ModelosDeVista;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace COT_0.Host.Views.Pantallas;

/// <summary>Pantalla USUARIOS: solo arma la vista y pasa clics, ESC y el velo; la lógica vive en UsersScreenViewModel</summary>
public sealed partial class UsuariosPage : Page
{
    public UsersScreenViewModel ViewModel { get; } = App.Services.GetRequiredService<UsersScreenViewModel>();

    public UsuariosPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e) => await ViewModel.LoadAsync();

    private void OnEscapeInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        args.Handled = true;
        ViewModel.EscapeCommand.Execute(null);
    }

    private void OnVeilTapped(object sender, TappedRoutedEventArgs e) => ViewModel.CloseOverlayCommand.Execute(null);

    private void OnUserClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is UserCardViewModel card) ViewModel.OpenCommand.Execute(card);
    }

    // Cada botón trae su tarjeta en el DataContext
    private void OnPhotoClick(object sender, RoutedEventArgs e) => Run(sender, ViewModel.ChangePhotoCommand);

    private void OnRenameClick(object sender, RoutedEventArgs e) => Run(sender, ViewModel.RenameCommand);

    private void OnPasswordClick(object sender, RoutedEventArgs e) => Run(sender, ViewModel.ChangePasswordCommand);

    private void OnDeleteClick(object sender, RoutedEventArgs e) => Run(sender, ViewModel.DeleteCommand);

    private static void Run(object sender, System.Windows.Input.ICommand command)
    {
        if (sender is FrameworkElement { DataContext: UserCardViewModel card }) command.Execute(card);
    }
}
