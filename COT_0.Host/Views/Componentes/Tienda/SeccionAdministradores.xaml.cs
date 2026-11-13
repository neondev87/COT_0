using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>Administradores: la página le pasa su AdminsViewModel; los botones y ENTER solo llaman a sus comandos</summary>
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

    private void OnNameKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != VirtualKey.Enter || ViewModel is null) return;
        e.Handled = true;
        if (ViewModel.CreateCommand.CanExecute(null)) ViewModel.CreateCommand.Execute(null);
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
