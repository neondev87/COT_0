// Aviso (InfoBar) de una sección de la pantalla Emotes: mensaje, tipo y si está abierto
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Cada sección tiene el suyo; el InfoBar se enlaza directo a estas propiedades.</summary>
public partial class NoticeViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isOpen;

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private InfoBarSeverity _severity = InfoBarSeverity.Informational;

    public void Show(string message, InfoBarSeverity severity)
    {
        Message = message;
        Severity = severity;
        IsOpen = true;
    }

    public void Hide() => IsOpen = false;
}
