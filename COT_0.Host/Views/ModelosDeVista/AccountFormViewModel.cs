// Recuadro de USUARIOS para CAMBIAR NOMBRE o CAMBIAR CONTRASEÑA de tu cuenta
using COT_0.Host.Models;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Usuarios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Un solo recuadro con dos modos; los dos piden primero tu respuesta secreta. La contraseña sigue las reglas nuevas
/// (el mensaje dice cada regla que falta); el nombre sigue las reglas de usuario y no se puede repetir. Done avisa el mensaje de éxito.
/// </summary>
public partial class AccountFormViewModel(AccountService accounts, string userName, bool isRename) : FormViewModel
{
    [ObservableProperty]
    private string _newName = userName;

    [ObservableProperty]
    private string _secretAnswer = string.Empty;

    [ObservableProperty]
    private string _newPassword = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    public event Action<string>? Done;

    public string UserName { get; } = userName;

    public bool IsRename { get; } = isRename;

    public bool IsPassword => !IsRename;

    public string Title => StoreTexts.Format(IsRename ? "Text.Users.Rename.Title" : "Text.Users.Password.Title", UserName.ToUpperInvariant());

    [RelayCommand]
    private async Task SubmitAsync()
    {
        IsErrorOpen = false;
        try
        {
            if (IsRename) await RenameAsync();
            else await ChangePasswordAsync();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            ShowError("Text.Error.Storage");
        }
    }

    private async Task RenameAsync()
    {
        var error = await accounts.RenameAsync(UserName, SecretAnswer, NewName);
        SecretAnswer = string.Empty;
        if (Check(error)) Done?.Invoke(StoreTexts.Format("Text.Users.Renamed", NewName));
    }

    private async Task ChangePasswordAsync()
    {
        var error = await accounts.ChangePasswordAsync(UserName, SecretAnswer, NewPassword, ConfirmPassword);
        SecretAnswer = string.Empty;
        if (Check(error, NewPassword)) Done?.Invoke(UiText.Get("Text.Users.PasswordChanged"));
    }
}
