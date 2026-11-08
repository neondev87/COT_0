// Diálogo de confirmación: solo recibe los textos; quien lo abre es ConfirmDialogService
using COT_0.Host.Models;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

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
