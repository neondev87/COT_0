// Motivos por los que se rechaza una acción de administradores (entrar, crear, regenerar o borrar)
namespace COT_0.Host.Models;

/// <summary>Cada valor tiene su texto "Text.Admin.Error.{valor}" en TextosAdmin.xaml.</summary>
public enum AdminError
{
    None,
    UserEmpty,
    UserInvalid,
    UserTooLong,
    Taken,
    Full,
    AlreadyCreated,
    NotFound,
    NotAllowed,
    WrongCredentials,
    QuestionRequired,
    QuestionMissing,
    DeleteRejected,
    Storage,
}
