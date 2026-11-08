// Estado y acciones del login: entra o registra (con pregunta secreta), recupera la contraseña y pasa al dashboard
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Validaciones;
using COT_0.Host.Services.Ventana;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Una sola cuenta de host: si todavía no existe, el mismo formulario sirve para crearla.</summary>
public partial class LoginViewModel : FormViewModel
{
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isSuccessOpen;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubtitleText), nameof(ButtonText), nameof(IsCredentialsVisible),
        nameof(IsQuestionVisible), nameof(IsRecoverVisible), nameof(IsForgotVisible), nameof(IsSubmitVisible))]
    private LoginStep _step;

    public LoginViewModel() => Recovery.Closed += OnRecoveryClosed;

    public SecurityQuestionViewModel Question { get; } = new();

    public RecoveryViewModel Recovery { get; } = new();

    public bool IsCredentialsVisible => Step is LoginStep.SignIn or LoginStep.Register;

    public bool IsQuestionVisible => Step is LoginStep.Register or LoginStep.SetupQuestion;

    public bool IsRecoverVisible => Step == LoginStep.Recover;

    public bool IsForgotVisible => Step == LoginStep.SignIn;

    public bool IsSubmitVisible => Step != LoginStep.Recover;

    public string SubtitleText => UiText.Get($"Text.Login.Subtitle.{Step}");

    public string ButtonText => UiText.Get($"Text.Login.Button.{Step}");

    /// <summary>Revisa si ya existe el host para decidir entre ENTRAR y REGISTRARME.</summary>
    public async Task LoadAsync()
    {
        try
        {
            Step = await UserService.HasUsersAsync() ? LoginStep.SignIn : LoginStep.Register;
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            ShowError("Text.Error.Storage");
        }
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        IsErrorOpen = IsSuccessOpen = false;
        try
        {
            var done = Step switch
            {
                LoginStep.Register => await RegisterAsync(),
                LoginStep.SetupQuestion => await SaveQuestionAsync(),
                _ => await SignInAsync(),
            };
            if (done) Navigator.ShowDashboard();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            ShowError("Text.Error.Storage");
        }
    }

    [RelayCommand]
    private async Task ForgotAsync()
    {
        IsErrorOpen = IsSuccessOpen = false;
        Step = LoginStep.Recover;
        await Recovery.StartAsync();
    }

    private async Task<bool> SignInAsync()
    {
        if (!Check(CredentialValidator.Validate(UserName, Password))) return false;

        if (!await UserService.LoginAsync(UserName, Password))
        {
            ShowError("Text.Error.WrongCredentials");
            Password = string.Empty;
            return false;
        }

        // Cuenta de antes de la recuperación: primero configura su pregunta
        if (!await UserService.NeedsSecurityQuestionAsync(UserName)) return true;
        Question.Clear();
        Step = LoginStep.SetupQuestion;
        return false;
    }

    private async Task<bool> RegisterAsync()
    {
        if (!Check(CredentialValidator.Validate(UserName, Password)) || !Check(Question.Validate())) return false;

        if (await UserService.RegisterAsync(UserName, Password, Question.QuestionKey, Question.Answer)) return true;
        ShowError("Text.Error.UserTaken");
        return false;
    }

    private async Task<bool> SaveQuestionAsync()
    {
        if (!Check(Question.Validate())) return false;

        await UserService.SetSecurityQuestionAsync(UserName, Question.QuestionKey, Question.Answer);
        return true;
    }

    private async void OnRecoveryClosed(bool changed)
    {
        Step = LoginStep.SignIn;
        if (!changed) return;

        Password = string.Empty;
        IsSuccessOpen = true;
        try
        {
            UserName = await PasswordRecoveryService.GetUserNameAsync();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            ShowError("Text.Error.Storage");
        }
    }
}
