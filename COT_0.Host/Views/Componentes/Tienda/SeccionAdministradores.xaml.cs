using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>Administradores: la página le pasa su AdminsViewModel; los botones de cada fila solo llaman a sus comandos</summary>
public sealed partial class SeccionAdministradores : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(AdminsViewModel), typeof(SeccionAdministradores), new PropertyMetadata(null));

    public SeccionAdministradores() => InitializeComponent();

    public AdminsViewModel ViewModel
    {
        get => (AdminsViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private void OnRegenerateClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: AdminRowViewModel row }) ViewModel.RegenerateCommand.Execute(row);
    }

    private void OnDeleteClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: AdminRowViewModel row }) ViewModel.DeleteCommand.Execute(row);
    }
}
