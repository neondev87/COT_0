// Diálogo de confirmación: solo recibe los textos; quien lo abre es ConfirmDialogService
using COT_0.HostWinUI.Models;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.HostWinUI.Views.Componentes;

public sealed partial class ConfirmacionDialogo : ContentDialog
{
    public ConfirmacionDialogo(ConfirmTexts texts)
    {
        InitializeComponent();
        MessageText.Text = texts.Message;
        PrimaryButtonText = texts.Confirm;
        CloseButtonText = texts.Cancel;
    }
}
