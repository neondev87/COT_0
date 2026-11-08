// Abre el diálogo de confirmación (sí/no) sobre la ventana principal
using COT_0.Host.Models;
using COT_0.Host.Services.Ventana;
using COT_0.Host.Views.Componentes;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Services.Dialogos;

/// <summary>Lo usa cualquier parte del Host que necesite un "¿seguro?". Esc o CANCELAR regresan false.</summary>
public class ConfirmDialogService
{
    // Solo se puede abrir un ContentDialog a la vez
    private bool _isOpen;

    public async Task<bool> ConfirmAsync(ConfirmTexts texts)
    {
        if (_isOpen || Navigator.XamlRoot is not { } root) return false;

        _isOpen = true;
        try
        {
            var dialog = new ConfirmacionDialogo(texts) { XamlRoot = root };
            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }
        finally
        {
            _isOpen = false;
        }
    }
}
