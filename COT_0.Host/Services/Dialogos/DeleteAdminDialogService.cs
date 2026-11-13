// Abre el diálogo ELIMINAR ADMIN (EliminarAdminDialogo) sobre la ventana principal y regresa lo escrito
using COT_0.Host.Models;
using COT_0.Host.Services.Ventana;
using COT_0.Host.Views.Componentes;
using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Services.Dialogos;

/// <summary>null si cancelaste (Esc o CANCELAR). Al cerrar, el diálogo olvida lo escrito.</summary>
public class DeleteAdminDialogService
{
    // Solo se puede abrir un ContentDialog a la vez
    private bool _isOpen;

    public async Task<AdminDeleteRequest?> AskAsync(string targetName, string questionKey, bool isSelf)
    {
        if (_isOpen || Navigator.XamlRoot is not { } root) return null;

        _isOpen = true;
        var viewModel = new DeleteAdminViewModel(targetName, questionKey, isSelf);
        try
        {
            var dialog = new EliminarAdminDialogo(viewModel) { XamlRoot = root };
            return await dialog.ShowAsync() == ContentDialogResult.Primary ? viewModel.ToRequest() : null;
        }
        finally
        {
            viewModel.Forget();
            _isOpen = false;
        }
    }
}
