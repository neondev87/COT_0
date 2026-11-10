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
    PasswordRules,
    PasswordTooLong,
    PasswordMismatch,
    QuestionEmpty,
    AnswerEmpty,
    AnswerInvalid,
    AnswerTooLong,
    UserTaken,
    UsersFull,
    AccountOutdated,
    WrongCredentials,
    NoSecurityQuestion,
    WrongAnswer,
    RenameFailed,
    NotAllowed,
}
