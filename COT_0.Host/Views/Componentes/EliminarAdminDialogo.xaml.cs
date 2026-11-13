using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

/// <summary>Diálogo para eliminar un admin: solo recibe su ViewModel; quien lo abre es DeleteAdminDialogService</summary>
public sealed partial class EliminarAdminDialogo : ContentDialog
{
    public EliminarAdminDialogo(DeleteAdminViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    public DeleteAdminViewModel ViewModel { get; }
}
