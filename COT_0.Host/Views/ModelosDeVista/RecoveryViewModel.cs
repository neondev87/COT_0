// "¿Olvidaste tu contraseña?": escribes tu usuario, contestas su pregunta y luego la contraseña nueva dos veces
using COT_0.Host.Services.Almacenamiento;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Validaciones;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Lo muestra el componente FormularioRecuperacion dentro del login.</summary>
public partial class RecoveryViewModel : FormViewModel
{
    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasQuestion), nameof(IsUserEditable), nameof(ButtonText))]
    private string _questionText = string.Empty;

    [ObservableProperty]
    private string _answer = string.Empty;

    [ObservableProperty]
    private string _newPassword = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonText), nameof(IsAnswerEditable))]
    private bool _isAnswerVerified;

    /// <summary>Paso 2: ya se encontró la pregunta del usuario.</summary>
    public bool HasQuestion => QuestionText.Length > 0;

    /// <summary>El usuario queda fijo en cuanto aparece su pregunta.</summary>
    public bool IsUserEditable => !HasQuestion;

    /// <summary>Ya verificada, la respuesta queda fija mientras escribes la contraseña nueva.</summary>
    public bool IsAnswerEditable => !IsAnswerVerified;

    public string ButtonText => UiText.Get(IsAnswerVerified ? "Text.Recovery.Save"
        : HasQuestion ? "Text.Recovery.Verify" : "Text.Recovery.Continue");

    /// <summary>Regresar al login: true si la contraseña sí se cambió.</summary>
    public event Action<bool>? Closed;

    /// <summary>Limpia el formulario; el usuario que ya estaba escrito en el login se deja.</summary>
    public void Start(string userName)
    {
        Answer = NewPassword = ConfirmPassword = QuestionText = string.Empty;
        UserName = userName;
        IsAnswerVerified = false;
        IsErrorOpen = false;
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        IsErrorOpen = false;
        try
        {
            if (IsAnswerVerified) await SavePasswordAsync();
            else if (HasQuestion) await VerifyAnswerAsync();
            else await FindQuestionAsync();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            ShowError("Text.Error.Storage");
        }
    }

    [RelayCommand]
    private void Back() => Closed?.Invoke(false);

    // Mismo mensaje si no existe o si no tiene pregunta: no revela qué usuarios hay
    private async Task FindQuestionAsync()
    {
        if (!Check(CredentialValidator.ValidateUserName(UserName))) return;

        var key = await PasswordRecoveryService.GetQuestionKeyAsync(UserName);
        if (key is null) ShowError("Text.Error.RecoveryFailed");
        else QuestionText = UiText.Get(key);
    }

    private async Task VerifyAnswerAsync()
    {
        if (!Check(SecurityAnswerValidator.ValidateAnswer(Answer))) return;

        IsAnswerVerified = await PasswordRecoveryService.VerifyAnswerAsync(UserName, Answer);
        if (!IsAnswerVerified) ShowError("Text.Error.WrongAnswer");
    }

    private async Task SavePasswordAsync()
    {
        if (!Check(CredentialValidator.ValidateNewPassword(NewPassword, ConfirmPassword))) return;

        if (await PasswordRecoveryService.ResetPasswordAsync(UserName, Answer, NewPassword)) Closed?.Invoke(true);
        else ShowError("Text.Error.WrongAnswer");
    }
}
