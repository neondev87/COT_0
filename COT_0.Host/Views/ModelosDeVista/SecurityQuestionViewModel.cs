// Pregunta secreta elegida y su respuesta (al registrarte o al configurarla en una cuenta vieja)
using COT_0.Host.Models;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Validaciones;
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Lo usa el componente CampoPregunta; la lista sale de Recovery:QuestionKeys.</summary>
public partial class SecurityQuestionViewModel : ObservableObject
{
    [ObservableProperty]
    private SecurityQuestionOption? _selectedQuestion;

    [ObservableProperty]
    private string _answer = string.Empty;

    public IReadOnlyList<SecurityQuestionOption> Questions { get; } =
        AppConfig.Settings.Recovery.QuestionKeys.Select(key => new SecurityQuestionOption(key, UiText.Get(key))).ToList();

    public string QuestionKey => SelectedQuestion?.Key ?? string.Empty;

    public CredentialError Validate() => SecurityAnswerValidator.Validate(SelectedQuestion?.Key, Answer);

    public void Clear()
    {
        SelectedQuestion = null;
        Answer = string.Empty;
    }
}
