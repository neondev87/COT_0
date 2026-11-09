// Estado y acciones del login: switch HOST | ADMIN, ENTRAR, registrar usuario, recuperar contraseña, pregunta secreta y paso al dashboard
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Validaciones;
using COT_0.Host.Services.Ventana;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Arranca en HOST (hasta Usuarios:Maximo usuarios). ADMIN tiene su propio formulario (LoginModeViewModel);
/// el switch solo se ve en el paso ENTRAR. ActiveUser guarda quién entró y en qué modo.
/// </summary>
public partial class LoginViewModel : FormViewModel
{
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isSuccessOpen;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNoUsersVisible))]
    private bool _hasUsers = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubtitleText), nameof(ButtonText), nameof(IsCredentialsVisible), nameof(IsQuestionVisible),
        nameof(IsRegisterVisible), nameof(IsRecoverVisible), nameof(IsSubmitVisible), nameof(IsNoUsersVisible), nameof(IsModeSwitchVisible))]
    private LoginStep _step;

    public LoginViewModel(LoginModeViewModel mode)
    {
        Mode = mode;
        Recovery.Closed += OnRecoveryClosed;
        Register.Closed += OnRegisterClosed;
        mode.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(LoginModeViewModel.IsAdminMode)) return;
            IsErrorOpen = IsSuccessOpen = false;
            Password = string.Empty;
            OnPropertyChanged(nameof(SubtitleText));
        };
    }

    /// <summary>Switch HOST | ADMIN y el formulario del modo ADMIN.</summary>
    public LoginModeViewModel Mode { get; }

    /// <summary>HOST | ADMIN solo en el paso ENTRAR (no en registro, recuperación ni pregunta).</summary>
    public bool IsModeSwitchVisible => Step == LoginStep.SignIn;

    public SecurityQuestionViewModel Question { get; } = new();

    public RecoveryViewModel Recovery { get; } = new();

    public RegisterViewModel Register { get; } = new();

    /// <summary>Usuario, contraseña, enlaces y ENTRAR.</summary>
    public bool IsCredentialsVisible => Step == LoginStep.SignIn;

    public bool IsQuestionVisible => Step == LoginStep.SetupQuestion;

    public bool IsRegisterVisible => Step == LoginStep.Register;

    public bool IsRecoverVisible => Step == LoginStep.Recover;

    /// <summary>ENTRAR o GUARDAR Y ENTRAR; registro y recuperación traen su propio botón.</summary>
    public bool IsSubmitVisible => Step is LoginStep.SignIn or LoginStep.SetupQuestion;

    /// <summary>Aviso de que todavía no hay nadie registrado.</summary>
    public bool IsNoUsersVisible => Step == LoginStep.SignIn && !HasUsers;

    public string SubtitleText => UiText.Get(Mode.IsAdminMode ? "Text.Login.Subtitle.Admin" : $"Text.Login.Subtitle.{Step}");

    public string ButtonText => UiText.Get(Step == LoginStep.SetupQuestion ? "Text.Login.Button.SetupQuestion" : "Text.Login.Button.SignIn");

    /// <summary>Siempre arranca en ENTRAR y en modo HOST; solo revisa si ya hay usuarios para el aviso.</summary>
    public async Task LoadAsync()
    {
        ActiveUser.SignOut();
        Step = LoginStep.SignIn;
        Mode.IsAdminMode = false;
        try
        {
            HasUsers = await UserService.HasUsersAsync();
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
            var done = Step == LoginStep.SetupQuestion ? await SaveQuestionAsync() : await SignInAsync();
            if (!done) return;
            // Solo en memoria, para la tienda; la interfaz no lo muestra
            ActiveUser.SignIn(UserName);
            Navigator.ShowDashboard();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            ShowError("Text.Error.Storage");
        }
    }

    /// <summary>ENTER: hace lo mismo que el botón principal del paso visible.</summary>
    [RelayCommand]
    private void Enter()
    {
        var command = Mode.IsAdminMode ? Mode.Admin.SubmitCommand : Step switch
        {
            LoginStep.Register => Register.SubmitCommand,
            LoginStep.Recover => Recovery.SubmitCommand,
            _ => (System.Windows.Input.ICommand)SubmitCommand,
        };
        if (command.CanExecute(null)) command.Execute(null);
    }

    [RelayCommand]
    private void Forgot()
    {
        IsErrorOpen = IsSuccessOpen = false;
        Recovery.Start(UserName);
        Step = LoginStep.Recover;
    }

    [RelayCommand]
    private async Task RegisterUserAsync()
    {
        IsErrorOpen = IsSuccessOpen = false;
        Step = LoginStep.Register;
        await Register.StartAsync();
    }

    private async Task<bool> SignInAsync()
    {
        if (!Check(CredentialValidator.ValidateSignIn(UserName, Password))) return false;

        // Contraseña equivocada o cuenta de antes de las reglas nuevas (no entra; se recrea en "Registrar usuario")
        if (!Check(await UserService.LoginAsync(UserName, Password)))
        {
            Password = string.Empty;
            return false;
        }

        // Primera vez que entra este usuario: primero configura su pregunta
        if (!await UserService.NeedsSecurityQuestionAsync(UserName)) return true;
        Question.Clear();
        Step = LoginStep.SetupQuestion;
        return false;
    }

    private async Task<bool> SaveQuestionAsync()
    {
        if (!Check(Question.Validate())) return false;

        return Check(await UserService.SetSecurityQuestionAsync(UserName, Question.QuestionKey, Question.Answer));
    }

    private void OnRecoveryClosed(bool changed)
    {
        Step = LoginStep.SignIn;
        if (!changed) return;

        UserName = Recovery.UserName;
        ShowSuccess("Text.Recovery.Done");
    }

    private void OnRegisterClosed(string? registeredUserName)
    {
        Step = LoginStep.SignIn;
        if (registeredUserName is null) return;

        UserName = registeredUserName;
        HasUsers = true;
        ShowSuccess("Text.Register.Done");
    }

    private void ShowSuccess(string key)
    {
        Password = string.Empty;
        SuccessMessage = UiText.Get(key);
        IsSuccessOpen = true;
    }
}
