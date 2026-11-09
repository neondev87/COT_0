// Muestra una sola vez la contraseña generada de un admin (ContrasenaAdminDialogo) sobre la ventana principal
using COT_0.Host.Services.Ventana;
using COT_0.Host.Views.Componentes;
using COT_0.Host.Views.ModelosDeVista;

namespace COT_0.Host.Services.Dialogos;

/// <summary>Termina cuando presionas ENTENDIDO; para entonces el diálogo ya soltó la contraseña.</summary>
public class AdminPasswordDialogService
{
    public async Task ShowAsync(string adminName, string password)
    {
        if (Navigator.XamlRoot is not { } root) return;

        var dialog = new ContrasenaAdminDialogo(new GeneratedPasswordViewModel(adminName, password)) { XamlRoot = root };
        await dialog.ShowAsync();
    }
}
