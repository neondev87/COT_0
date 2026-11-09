using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes.Tienda;

/// <summary>Sección USUARIOS: la página le pasa su StoreUsersViewModel; el clic en una tarjeta solo llama a su comando</summary>
public sealed partial class SeccionUsuarios : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(StoreUsersViewModel), typeof(SeccionUsuarios), new PropertyMetadata(null));

    public SeccionUsuarios() => InitializeComponent();

    public StoreUsersViewModel ViewModel
    {
        get => (StoreUsersViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private void OnUserClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is UserCardViewModel card) ViewModel.OpenCommand.Execute(card);
    }
}
