// Motivos por los que se rechaza lo que escribiste en el login o en la recuperación
namespace COT_0.Host.Models;

/// <summary>Cada valor tiene su texto "Text.Error.{valor}" en Textos.xaml.</summary>
public enum CredentialError
{
    None,
    UserEmpty,
    UserInvalid,
    UserTooLong,
    PasswordEmpty,
    PasswordInvalid,
    PasswordTooLong,
    PasswordMismatch,
    QuestionEmpty,
    AnswerEmpty,
    AnswerTooLong,
    UserTaken,
    UsersFull,
}
