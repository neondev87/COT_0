// Base de los formularios del login: mensaje de error para el InfoBar
using COT_0.HostWinUI.Models;
using COT_0.HostWinUI.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.HostWinUI.Views.ModelosDeVista;

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
        return false;
    }

    protected void ShowError(string key)
    {
        ErrorMessage = UiText.Get(key);
        IsErrorOpen = true;
    }
}
