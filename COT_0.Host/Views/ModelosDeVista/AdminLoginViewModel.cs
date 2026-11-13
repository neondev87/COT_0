// Modo ADMIN del login: ENTRAR con usuario y contraseña de admin, configurar su pregunta secreta la primera vez o crear el primer admin
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
/// Si un admin entra sin pregunta secreta (nuevo o de antes), primero la configura y luego entra.
/// Si falla, el mensaje no dice si el usuario existe.
/// </summary>
public partial class AdminLoginViewModel(AdminService admins, AdminPasswordDialogService passwordDialog) : FormViewModel
{
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCreateVisible), nameof(IsPasswordVisible), nameof(ButtonText))]
    private bool _hasAdmins = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCredentialsVisible), nameof(IsPasswordVisible), nameof(ButtonText))]
    private bool _isQuestionStep;

    [ObservableProperty]
    private bool _isSuccessOpen;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    /// <summary>Entró un admin (su nombre).</summary>
    public event Action<string>? SignedIn;

    public SecurityQuestionViewModel Question { get; } = new();

    public bool IsCreateVisible => !HasAdmins;

    public bool IsCredentialsVisible => !IsQuestionStep;

    public bool IsPasswordVisible => HasAdmins && !IsQuestionStep;

    public string ButtonText => UiText.Get(IsQuestionStep ? "Text.Login.Button.SetupQuestion"
        : HasAdmins ? "Text.Login.Button.SignIn" : "Text.Admin.CreateFirst.Button");

    /// <summary>Al elegir ADMIN: revisa si ya hay admins (si admin.json estaba dañado, lo avisa).</summary>
    public async Task StartAsync()
    {
        IsErrorOpen = IsSuccessOpen = false;
        Password = string.Empty;
        LeaveQuestionStep();
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

    /// <summary>Al regresar a HOST: si dejó la pregunta a medias, no entra.</summary>
    public void LeaveQuestionStep()
    {
        IsQuestionStep = false;
        Question.Clear();
        admins.CancelPendingQuestion();
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        IsErrorOpen = IsSuccessOpen = false;
        try
        {
            if (IsQuestionStep) await SaveQuestionAsync();
            else if (HasAdmins) await SignInAsync();
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
        else if (error == AdminError.QuestionRequired) EnterQuestionStep();
        else ShowMessage(AdminTexts.Error(error));
    }

    private void EnterQuestionStep()
    {
        Question.Clear();
        IsQuestionStep = true;
        SuccessMessage = UiText.Get("Text.Admin.Question.Required");
        IsSuccessOpen = true;
    }

    private async Task SaveQuestionAsync()
    {
        if (!Check(Question.Validate())) return;
        if (!Check(await admins.SetQuestionAsync(UserName, Question.QuestionKey, Question.Answer))) return;

        Question.Clear();
        IsQuestionStep = false;
        SignedIn?.Invoke(UserName);
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
