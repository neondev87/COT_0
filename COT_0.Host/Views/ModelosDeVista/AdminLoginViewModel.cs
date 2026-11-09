// Modo ADMIN del login: ENTRAR con usuario y contraseña de admin, o crear el primer admin si todavía no hay ninguno
using COT_0.Host.Models;
using COT_0.Host.Services.Administradores;
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Dialogos;
using COT_0.Host.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Primer admin: solo se escribe el nombre; la contraseña la genera la app y se muestra una vez.
/// Después se entra con ella como cualquier admin. Si falla, el mensaje no dice si el usuario existe.
/// </summary>
public partial class AdminLoginViewModel(AdminService admins, AdminPasswordDialogService passwordDialog) : FormViewModel
{
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCreateVisible), nameof(ButtonText))]
    private bool _hasAdmins = true;

    [ObservableProperty]
    private bool _isSuccessOpen;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    /// <summary>Entró un admin (su nombre).</summary>
    public event Action<string>? SignedIn;

    public bool IsCreateVisible => !HasAdmins;

    public string ButtonText => UiText.Get(HasAdmins ? "Text.Login.Button.SignIn" : "Text.Admin.CreateFirst.Button");

    /// <summary>Al elegir ADMIN: revisa si ya hay admins (si admin.json estaba dañado, lo avisa).</summary>
    public async Task StartAsync()
    {
        IsErrorOpen = IsSuccessOpen = false;
        Password = string.Empty;
        try
        {
            HasAdmins = await admins.HasAdminsAsync();
            if (admins.TakeRecreatedNotice()) ShowError("Text.Admin.Recreated");
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            ShowError("Text.Admin.Error.Storage");
        }
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        IsErrorOpen = IsSuccessOpen = false;
        try
        {
            if (HasAdmins) await SignInAsync();
            else await CreateFirstAsync();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            ShowError("Text.Admin.Error.Storage");
        }
    }

    private async Task SignInAsync()
    {
        var error = await admins.LoginAsync(UserName, Password);
        Password = string.Empty;
        if (error == AdminError.None) SignedIn?.Invoke(UserName);
        else ShowMessage(AdminTexts.Error(error));
    }

    private async Task CreateFirstAsync()
    {
        var result = await admins.CreateFirstAsync(UserName);
        if (result.Password is not { } password)
        {
            if (result.Error == AdminError.AlreadyCreated) HasAdmins = true;
            ShowMessage(AdminTexts.Error(result.Error));
            return;
        }

        await passwordDialog.ShowAsync(UserName, password);
        HasAdmins = true;
        SuccessMessage = UiText.Get("Text.Admin.CreateFirst.Done");
        IsSuccessOpen = true;
    }

    private void ShowMessage(string message)
    {
        ErrorMessage = message;
        IsErrorOpen = true;
    }
}
