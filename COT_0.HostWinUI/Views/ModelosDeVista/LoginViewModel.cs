// Estado y acciones del login: valida, entra o registra y pasa al dashboard
using System.Text.Json;
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Usuarios;
using COT_0.HostWinUI.Services.Validaciones;
using COT_0.HostWinUI.Services.Ventana;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>Si todavía no hay host registrado, el mismo formulario sirve para crear la cuenta.</summary>
public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isErrorOpen;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubtitleText), nameof(ButtonText))]
    private bool _isRegisterMode;

    public string SubtitleText => UiText.Get(IsRegisterMode ? "Text.Login.SubtitleRegister" : "Text.Login.Subtitle");

    public string ButtonText => UiText.Get(IsRegisterMode ? "Text.Login.Register" : "Text.Login.Enter");

    /// <summary>Revisa si ya existe un host para decidir entre ENTRAR y REGISTRARME.</summary>
    public async Task LoadAsync()
    {
        try
        {
            IsRegisterMode = !await UserService.HasUsersAsync();
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            ShowError("Text.Error.Storage");
        }
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        IsErrorOpen = false;

        // Primero el formato: si no pasa, ni siquiera se intenta el login
        var error = CredentialValidator.Validate(UserName, Password);
        if (error != CredentialError.None)
        {
            ShowError($"Text.Error.{error}");
            return;
        }

        try
        {
            if (IsRegisterMode ? await RegisterAsync() : await UserService.LoginAsync(UserName, Password))
            {
                Navigator.ShowDashboard();
                return;
            }
            if (!IsRegisterMode) ShowError("Text.Error.WrongCredentials");
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            ShowError("Text.Error.Storage");
        }
        Password = string.Empty;
    }

    private async Task<bool> RegisterAsync()
    {
        if (await UserService.RegisterAsync(UserName, Password)) return true;
        ShowError("Text.Error.UserTaken");
        return false;
    }

    private void ShowError(string key)
    {
        ErrorMessage = UiText.Get(key);
        IsErrorOpen = true;
    }
}
