// Diálogo ELIMINAR ADMIN: respuesta secreta del admin que se va y contraseña del admin que elimina
using System.Globalization;
using COT_0.Host.Models;
using COT_0.Host.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// Los dos campos son PasswordBox (no se ven). ELIMINAR se enciende cuando ambos tienen algo;
/// la revisión real contra los hashes la hace AdminService. Forget suelta lo escrito al cerrar.
/// </summary>
public partial class DeleteAdminViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfirm))]
    private string _targetAnswer = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfirm))]
    private string _actorPassword = string.Empty;

    public DeleteAdminViewModel(string targetName, string questionKey, bool isSelf)
    {
        var name = targetName.ToUpperInvariant();
        Title = string.Format(CultureInfo.CurrentCulture, UiText.Get("Text.Admins.DeleteDialog.Title"), name);
        AnswerLabel = string.Format(CultureInfo.CurrentCulture, UiText.Get("Text.Admins.DeleteDialog.AnswerLabel"), name);
        QuestionText = UiText.Get(questionKey);
        Warning = UiText.Get(isSelf ? "Text.Admins.DeleteDialog.SelfWarning" : "Text.Admins.DeleteDialog.Warning");
    }

    public string Title { get; }

    public string AnswerLabel { get; }

    public string QuestionText { get; }

    public string Warning { get; }

    public bool CanConfirm => TargetAnswer.Length > 0 && ActorPassword.Length > 0;

    /// <summary>Lo escrito, listo para que AdminService lo compare; null si falta algo.</summary>
    public AdminDeleteRequest? ToRequest() => CanConfirm ? new AdminDeleteRequest(TargetAnswer, ActorPassword) : null;

    public void Forget()
    {
        TargetAnswer = string.Empty;
        ActorPassword = string.Empty;
    }
}
