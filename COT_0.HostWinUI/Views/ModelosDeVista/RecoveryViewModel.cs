// "¿Olvidaste tu contraseña?": primero contestas la pregunta, luego escribes la contraseña nueva dos veces
using COT_0.HostWinUI.Services.Almacenamiento;
using COT_0.HostWinUI.Services.Textos;
using COT_0.HostWinUI.Services.Usuarios;
using COT_0.HostWinUI.Services.Validaciones;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

/// <summary>Lo muestra el componente FormularioRecuperacion dentro del login.</summary>
public partial class RecoveryViewModel : FormViewModel
{
    [ObservableProperty]
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

    /// <summary>Ya verificada, la respuesta queda fija mientras escribes la contraseña nueva.</summary>
    public bool IsAnswerEditable => !IsAnswerVerified;

    public string ButtonText => UiText.Get(IsAnswerVerified ? "Text.Recovery.Save" : "Text.Recovery.Verify");

    /// <summary>Regresar al login: true si la contraseña sí se cambió.</summary>
    public event Action<bool>? Closed;

    /// <summary>Limpia el formulario y carga la pregunta guardada.</summary>
    public async Task StartAsync()
    {
        Answer = NewPassword = ConfirmPassword = QuestionText = string.Empty;
        IsAnswerVerified = false;
        IsErrorOpen = false;
        try
        {
            var key = await PasswordRecoveryService.GetQuestionKeyAsync();
            if (key is null) ShowError("Text.Error.NoQuestion");
            else QuestionText = UiText.Get(key);
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
        try
        {
            if (IsAnswerVerified) await SavePasswordAsync();
            else await VerifyAnswerAsync();
        }
        catch (Exception ex) when (JsonStore.IsStorageError(ex))
        {
            ShowError("Text.Error.Storage");
        }
    }

    [RelayCommand]
    private void Back() => Closed?.Invoke(false);

    private async Task VerifyAnswerAsync()
    {
        if (string.IsNullOrEmpty(QuestionText)) return;
        if (!Check(SecurityAnswerValidator.ValidateAnswer(Answer))) return;

        IsAnswerVerified = await PasswordRecoveryService.VerifyAnswerAsync(Answer);
        if (!IsAnswerVerified) ShowError("Text.Error.WrongAnswer");
    }

    private async Task SavePasswordAsync()
    {
        if (!Check(CredentialValidator.ValidateNewPassword(NewPassword, ConfirmPassword))) return;

        if (await PasswordRecoveryService.ResetPasswordAsync(Answer, NewPassword)) Closed?.Invoke(true);
        else ShowError("Text.Error.WrongAnswer");
    }
}
