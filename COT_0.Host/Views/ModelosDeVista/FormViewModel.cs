// Base de los formularios del login: mensaje de error para el InfoBar
using System.Globalization;
using COT_0.Host.Models;
using COT_0.Host.Services.Textos;
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Validaciones;
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Login y recuperación muestran sus errores igual: texto "Text.Error.{error}" en un InfoBar.</summary>
public abstract partial class FormViewModel : ObservableObject
{
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isErrorOpen;

    /// <summary>true si no hay error; si lo hay, lo muestra.</summary>
    protected bool Check(CredentialError error)
    {
        if (error == CredentialError.None) return true;
        ShowError($"Text.Error.{error}");
        // Los "demasiado largo" dicen el límite de Inputs:MaxCaracteres y "ya no caben" el de Usuarios:Maximo
        int? limit = error switch
        {
            CredentialError.UserTooLong or CredentialError.PasswordTooLong or CredentialError.AnswerTooLong => InputLimits.MaxCharacters,
            CredentialError.UsersFull => HostAccounts.Maximum,
            _ => null,
        };
        if (limit is not null) ErrorMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessage, limit);
        return false;
    }

    protected void ShowError(string key)
    {
        ErrorMessage = UiText.Get(key);
        IsErrorOpen = true;
    }
}
