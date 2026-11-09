// "Registrar usuario": nombre, contraseña y repetirla; la pregunta secreta se pide después de entrar
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Validaciones;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Lo muestra el componente FormularioRegistro dentro del login.</summary>
public partial class RegisterViewModel : FormViewModel
{
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    /// <summary>Regresar al login: el nombre registrado, o null si cancelaste.</summary>
    public event Action<string?>? Closed;

    /// <summary>Limpia el formulario y avisa de una vez si ya no caben más usuarios.</summary>
    public async Task StartAsync()
    {
        UserName = Password = ConfirmPassword = string.Empty;
        IsErrorOpen = false;
        try
        {
            if (await UserService.IsFullAsync()) Check(CredentialError.UsersFull);
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            ShowError("Text.Error.Storage");
        }
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        IsErrorOpen = false;
        if (!Check(CredentialValidator.ValidateRegistration(UserName, Password, ConfirmPassword), Password)) return;
        try
        {
            if (Check(await UserService.RegisterAsync(UserName, Password), Password)) Closed?.Invoke(UserName);
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            ShowError("Text.Error.Storage");
        }
    }

    [RelayCommand]
    private void Back() => Closed?.Invoke(null);
}
