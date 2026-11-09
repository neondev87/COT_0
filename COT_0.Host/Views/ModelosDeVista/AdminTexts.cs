// Mensajes de errores de administradores ya con sus números (largo máximo y Admin:Maximo)
using System.Globalization;
using COT_0.Host.Models;
using COT_0.Host.Services.Administradores;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Validaciones;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>"Text.Admin.Error.{error}"; {0} es el largo máximo o el máximo de admins según el error.</summary>
public static class AdminTexts
{
    public static string Error(AdminError error)
    {
        object value = error == AdminError.Full ? AdminService.Maximum : InputLimits.MaxCharacters;
        return string.Format(CultureInfo.CurrentCulture, UiText.Get($"Text.Admin.Error.{error}"), value);
    }
}
