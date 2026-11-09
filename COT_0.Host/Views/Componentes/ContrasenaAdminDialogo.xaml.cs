using COT_0.Host.Views.ModelosDeVista;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.Componentes;

/// <summary>Contraseña generada de un admin: solo pasa ENTENDIDO al ViewModel; Esc o cualquier otro cierre se cancela</summary>
public sealed partial class ContrasenaAdminDialogo : ContentDialog
{
    public ContrasenaAdminDialogo(GeneratedPasswordViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        Closing += OnClosing;
    }

    public GeneratedPasswordViewModel ViewModel { get; }

    private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args) =>
        args.Cancel = args.Result != ContentDialogResult.Primary || !ViewModel.TryClose();
}
