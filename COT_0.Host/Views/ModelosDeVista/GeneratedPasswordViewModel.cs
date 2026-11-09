// Diálogo de la contraseña generada de un admin: VER / OCULTAR, COPIAR y ENTENDIDO (pide confirmar si no la copiaste)
using System.Globalization;
using COT_0.Host.Services.Administradores;
using COT_0.Host.Services.Configuracion;
using COT_0.Host.Services.Textos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>
/// La contraseña en claro vive solo aquí mientras el diálogo está abierto; Forget la suelta al cerrar
/// (no se escribe en logs, archivos ni docs). Arranca oculta con puntos del mismo largo.
/// </summary>
public partial class GeneratedPasswordViewModel : ObservableObject
{
    private const char MaskChar = '•';

    private string? _password;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayText), nameof(RevealText))]
    private bool _isRevealed;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CopyText))]
    private bool _isCopied;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CloseText))]
    private bool _isConfirmPending;

    [ObservableProperty]
    private bool _isCopyFailed;

    public GeneratedPasswordViewModel(string adminName, string password)
    {
        _password = password;
        Title = string.Format(CultureInfo.CurrentCulture, UiText.Get("Text.Admin.Password.Title"), adminName.ToUpperInvariant());
    }

    public string Title { get; }

    /// <summary>"Se borra del portapapeles en N segundos" (Admin:ClipboardClearSeconds).</summary>
    public string ClipboardHint { get; } = string.Format(CultureInfo.CurrentCulture,
        UiText.Get("Text.Admin.Password.ClipboardHint"), AppConfig.Settings.Admin.ClipboardClearSeconds);

    public string DisplayText => _password is null ? string.Empty : IsRevealed ? _password : new string(MaskChar, _password.Length);

    public string RevealText => UiText.Get(IsRevealed ? "Text.Admin.Password.Hide" : "Text.Admin.Password.Show");

    public string CopyText => UiText.Get(IsCopied ? "Text.Admin.Password.Copied" : "Text.Admin.Password.Copy");

    public string CloseText => UiText.Get(IsConfirmPending ? "Text.Admin.Password.ConfirmClose" : "Text.Admin.Password.Close");

    [RelayCommand]
    private void ToggleReveal() => IsRevealed = !IsRevealed;

    [RelayCommand]
    private void Copy()
    {
        if (_password is null) return;
        IsCopied = ClipboardGuard.CopySecret(_password);
        IsCopyFailed = !IsCopied;
    }

    /// <summary>ENTENDIDO: si no la copiaste, la primera vez solo pide confirmar; true si ya se puede cerrar.</summary>
    public bool TryClose()
    {
        if (!IsCopied && !IsConfirmPending)
        {
            IsConfirmPending = true;
            return false;
        }
        Forget();
        return true;
    }

    // .NET no deja borrar un string en su lugar: se suelta la única referencia y se limpia lo que se ve
    private void Forget()
    {
        _password = null;
        IsRevealed = false;
        OnPropertyChanged(nameof(DisplayText));
    }
}
